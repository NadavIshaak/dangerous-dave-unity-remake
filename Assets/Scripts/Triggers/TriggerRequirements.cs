namespace Triggers
{
    public struct TriggerRequirements
    {
        public string Message;
        public bool HasGun;
        public bool HasJetPack;
        public int RequiredScore;
        public int RequiredLevel;
        public TriggerRequirements(string message, bool someBool, bool someOtherBool, int requiredScore, int requiredLevel)
        {
            Message = message;
            HasGun = someBool;
            HasJetPack = someOtherBool;
            RequiredScore = requiredScore;
            RequiredLevel = requiredLevel;
        }
    }
}