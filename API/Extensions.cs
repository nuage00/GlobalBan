using System;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace fr34kyn01535.GlobalBan.API
{
    public static class Extensions
    {
        public static ushort GetUShortWithSuffix([NotNull] this IEnumerable<string> args, string suffix, out int index)
        {
            var list = args.ToList();

            for (index = 0; index < list.Count; index++)
            {
                var element = list[index];
                var elementNoSuffix = element.Substring(0, element.Length - suffix.Length);
                if (element.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) && ushort.TryParse(elementNoSuffix, out var output))
                    return output;
            }

            index = -1;
            return ushort.MinValue;
        }

        public static uint GetUInt([NotNull] this IEnumerable<string> args, out int index)
        {
            var list = args.ToList();

            for (index = 0; index < list.Count; index++)
                if (uint.TryParse(list[index], out var output))
                    return output;

            index = -1;
            return uint.MinValue;
        }

        public static IRocketPlayer GetIRocketPlayer([NotNull] this IEnumerable<string> args, out int index)
        {
            IRocketPlayer output = null;
            index = args.ToList().FindIndex(k =>
            {
                output = UnturnedPlayer.FromName(k);
                if (output == null && ulong.TryParse(k, out var id) && id > 76561197960265728)
                    output = new RocketPlayer(id.ToString());

                return output != null;
            });
            return output;
        }
    }
}