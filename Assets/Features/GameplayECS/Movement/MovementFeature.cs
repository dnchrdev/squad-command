using Features.GameplayECS.Movement.Systems;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Movement
{
    public static class MovementFeature
    {
        public static void Add(SystemsGroup group)
        {
            group.AddSystem(new ResetVelocitySystem());
            group.AddSystem(new ApplyMoveSpeedSystem());
        }
    }
}