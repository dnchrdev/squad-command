namespace Features.Command.Entities
{
    public sealed class CommandSettings
    {
        public float SingleSelectThreshold { get; }

        public CommandSettings(float singleSelectThreshold)
        {
            SingleSelectThreshold = singleSelectThreshold;
        }
    }
}