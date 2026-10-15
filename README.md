# Squad Command

Учебный Unity-проект: игровая логика на ECS (Morpeh), управляющий слой на OOP по Clean Architecture, unit-тесты для обеих частей. Управление отрядом в духе RTS: выделение юнитов и приказ на движение строем.

## Демо
https://github.com/user-attachments/assets/b7d50b8c-4ddb-4b6f-8f7a-82e0bc879af3

## Цель

- Разобраться с ECS: как делить логику на фичи и системы и как фичи взаимодействуют.
- Совместить ECS с OOP: где проходит граница и как по ней передаются данные.
- Освоить unit-тестирование use case'ов и ECS-систем.

## Управление

| Действие | Управление |
|---|---|
| Режим выделения | `Q` |
| Режим движения | `W` |
| Выделить юнита / рамкой | ЛКМ: клик / зажать и тянуть |
| Добавить к выделению | `Shift` + ЛКМ |
| Убрать из выделения | `Ctrl` + ЛКМ |
| Приказ на движение | в режиме движения зажать ЛКМ в точке назначения и тянуть — задаёт направление и ширину строя |
| Перемещение камеры | ПКМ + мышь |
| Вращение камеры | `Space` + мышь |
| Зум | колесо мыши |

## Стек

Unity · C# · Morpeh · Zenject · UniTask · R3 · Addressables · Input System · NUnit

## Архитектура

Проект разделён на две части с одной точкой контакта.

- **OOP** превращает ввод игрока в команды, понятные ECS: режимы выделения и движения, рамка, построение строя, камера, UI. Каждая фича разделена на слои `Entities → UseCases → InterfaceAdapters → Infrastructure`, зависимости направлены внутрь.
- **ECS** исполняет команды: юниты, слоты строя, навигация, движение, столкновения. ECS-фича — группа систем, у каждого компонента один писатель:
  - `Components/` — приватное состояние фичи;
  - `Requests/` — публичный вход: другие фичи создают реквесты, чтобы попросить фичу что-то сделать;
  - `Outputs/` — публичный выход: пишет только сама фича, другие только читают.

Правила границы:

- OOP пишет в ECS только реквестами, а читает только снимки данных.
- Из Morpeh OOP знает только тип `Entity` — как идентификатор юнита.
- Системы ECS не вызывают OOP-код. Сервисы Unity (NavMesh, Addressables, Random) доступны системам только через порты.

Порядок ECS-фаз задаётся в одном месте — `GameRunner`:
`Spawning → UnitView → Synchronization (чтение) → Selection → MoveCommand → Navigation → Movement → Physics → Synchronization`.

## Структура

```
Assets/
├── Features/
│   ├── Camera/ Command/ Input/ UI/ GameLifecycle/   OOP-фичи (Clean Architecture)
│   ├── Shared/                                      порты между фичами и между OOP и ECS
│   ├── Core/                                        пул объектов
│   └── GameplayECS/
│       ├── Selection/ MoveCommand/ Navigation/
│       │   Movement/ Physics/ Synchronization/
│       │   UnitView/ Spawning/                      ECS-фичи
│       ├── Common/                                  компоненты без одного владельца (UnitTag)
│       ├── Facade/                                  шлюзы OOP ↔ ECS
│       └── Runner/                                  GameRunner
└── Tests/EditMode/                                  unit-тесты
```

## Технические решения

| Решение | Где смотреть |
|---|---|
| Назначение юнитам слотов строя венгерским алгоритмом: минимальная суммарная дистанция, пути не пересекаются | [`SlotAssignmentSolver`](Assets/Features/GameplayECS/MoveCommand/Destination/Systems/SlotAssignmentSolver.cs) |
| Адаптивная сетка строя: форма зависит от перетаскивания, от квадрата до шеренги | [`ApplyUpdateMovePreviewFormationRequestSystem`](Assets/Features/GameplayECS/MoveCommand/Preview/Systems/ApplyUpdateMovePreviewFormationRequestSystem.cs) |
| Собственное разрешение столкновений; вся цепочка движения в `FixedUpdate` | [`ResolveCollisionsSystem`](Assets/Features/GameplayECS/Physics/Systems/ResolveCollisionsSystem.cs) |
| Асинхронная загрузка вьюх через Addressables без `await` в системах: очередь, тег загрузки, пул | [`CreateViewSystem`](Assets/Features/GameplayECS/UnitView/Systems/CreateViewSystem.cs) |

## Тесты

Около 200 unit-тестов в EditMode, без сцены и Play Mode.

- **OOP** — тестируются Entities и UseCases: в них правила и сценарии. InterfaceAdapters и Infrastructure не тестируются: это тонкие обёртки, которые только передают вызовы в Unity.
- **ECS** — тестируются системы: в них правила мира. Вьюхи, Rigidbody, NavMesh и Addressables доступны системам только через интерфейсы, поэтому системы проверяются на настоящем `World` Morpeh без сцены.
