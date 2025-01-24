namespace Triggers
{
    public struct TriggerRequirements
    {
        public string Message;
        public readonly bool HasGun;
        public readonly bool HasJetPack;
        public readonly bool HasTrophy;
        public readonly int RequiredScore;
        public readonly int RequiredLevel;

        public TriggerRequirements(string message, bool someBool, bool someOtherBool, int requiredScore,
            int requiredLevel, bool hasTrophy)
        {
            Message = message;
            HasGun = someBool;
            HasJetPack = someOtherBool;
            HasTrophy = hasTrophy;
            RequiredScore = requiredScore;
            RequiredLevel = requiredLevel;
        }
    }
}