namespace Triggers
{
    public struct TriggerRequirements
    {
        public string Message;
        public bool HasGun;
        public bool HasJetPack;
        public bool HasTrophy;
        public int RequiredScore;
        public int RequiredLevel;
        public TriggerRequirements(string message, bool someBool, bool someOtherBool, int requiredScore, int requiredLevel, bool hasTrophy)
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