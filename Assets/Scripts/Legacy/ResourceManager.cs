namespace Legacy
{
    public static class ResourceManager
    {
        public static float humanResouces;
        public static float industrialResources;

        public static float hrBonus;
        public static float irBonus;

        public static bool Consume(bool hr, float amount)
        {
            if (hr)
            {
                amount *= hrBonus;
                if (humanResouces >= amount)
                {
                    humanResouces -= amount;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                amount *= irBonus;
                if (industrialResources >= amount)
                {
                    industrialResources -= amount;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
