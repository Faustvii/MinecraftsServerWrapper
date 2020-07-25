using System;

namespace MinecraftServerWrapper.Minecraft
{
    public static class MinecraftPatternMatchingExtensions
    {
        public static bool IsPlayerDeathMessage(this string str) => (str.IsArrowDeath() || str.IsCactusDeath()
            || str.IsDrownedDeath() || str.IsElytraDeath() || str.IsExplosionDeath() || str.IsFallingDeath()
            || str.IsFallingBlockDeath() || str.IsFireDeath() || str.IsRocketDeath() || str.IsLavaDeath()
            || str.IsLightningDeath() || str.IsMagmaDeath() || str.IsMagicDeath() || str.IsPvPvEDeath()
            || str.IsStarvationDeath() || str.IsSuffocationDeath() || str.IsBerryDeath() || str.IsThornsDeath()
            || str.IsTridentDeath() || str.IsVoidDeath() || str.IsWitherDeath() || str.IsMiscDeath())
            && !str.IsNPCDeath();

        private static bool IsArrowDeath(this string str) => str.ContainsIgnoreCase("was shot by");
        private static bool IsCactusDeath(this string str) => str.ContainsIgnoreCase("was pricked to death") || str.ContainsIgnoreCase("walked into a cactus");
        private static bool IsDrownedDeath(this string str) => str.ContainsIgnoreCase("drowned");
        private static bool IsElytraDeath(this string str) => str.ContainsIgnoreCase("kinetic energy");
        private static bool IsExplosionDeath(this string str) => str.ContainsIgnoreCase("blew up") || str.ContainsIgnoreCase("was blown up") || str.ContainsIgnoreCase("intentional game design");
        private static bool IsFallingDeath(this string str) => str.ContainsIgnoreCase("hit the ground") || str.ContainsIgnoreCase("fell off") || str.ContainsIgnoreCase("fell from") || str.ContainsIgnoreCase("fell while climbing");
        private static bool IsFallingBlockDeath(this string str) => str.ContainsIgnoreCase("was squashed by a falling");
        private static bool IsFireDeath(this string str) => str.ContainsIgnoreCase("burned to death") || str.ContainsIgnoreCase("went up in flames") || str.ContainsIgnoreCase("walked into fire") || str.ContainsIgnoreCase("burnt to a crisp");
        private static bool IsRocketDeath(this string str) => str.ContainsIgnoreCase("went off with a bang");
        private static bool IsLavaDeath(this string str) => str.ContainsIgnoreCase("tried to swim in lava");
        private static bool IsLightningDeath(this string str) => str.ContainsIgnoreCase("was struck by lightning");
        private static bool IsMagmaDeath(this string str) => str.ContainsIgnoreCase("discovered the floor was lava") || str.ContainsIgnoreCase("walked into danger zone");
        private static bool IsMagicDeath(this string str) => str.ContainsIgnoreCase("killed") && str.ContainsIgnoreCase("magic");
        private static bool IsPvPvEDeath(this string str) => str.ContainsIgnoreCase("was slain by") || str.ContainsIgnoreCase("was fireballed by") || str.ContainsIgnoreCase("was stung to death");
        private static bool IsStarvationDeath(this string str) => str.ContainsIgnoreCase("starved to death");
        private static bool IsSuffocationDeath(this string str) => str.ContainsIgnoreCase("suffocated in a") || str.ContainsIgnoreCase("was squished") || str.ContainsIgnoreCase("was squashed by");
        private static bool IsBerryDeath(this string str) => str.ContainsIgnoreCase("was poked to death by a sweet");
        private static bool IsThornsDeath(this string str) => str.ContainsIgnoreCase("trying to hurt") && str.ContainsIgnoreCase("killed");
        private static bool IsTridentDeath(this string str) => str.ContainsIgnoreCase("was impaled by");
        private static bool IsVoidDeath(this string str) => str.ContainsIgnoreCase("fell out of the world") || str.ContainsIgnoreCase("live in the same world as");
        private static bool IsWitherDeath(this string str) => str.ContainsIgnoreCase("withered away");
        private static bool IsMiscDeath(this string str) => str.ContainsIgnoreCase("was pummeled by") || str.ContainsIgnoreCase("was roasted in dragon") || str.ContainsIgnoreCase("was doomed to fall") || str.ContainsIgnoreCase("fell too far") || str.ContainsIgnoreCase("died") || str.ContainsIgnoreCase("was killed by");
        private static bool IsVillagerDeath(this string str) => str.ContainsIgnoreCase("class_1646");
        private static bool IsNPCDeath(this string str) => str.ContainsIgnoreCase("serverlevel");

        public static bool ContainsIgnoreCase(this string str, string matchPattern) => str.Contains(matchPattern, StringComparison.OrdinalIgnoreCase);
    }
}