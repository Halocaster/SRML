﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML
{
    internal static class DependencyChecker
    {
        public static bool CheckDependencies(HashSet<SRModLoader.ProtoMod> mods)
        {
            foreach (var mod in mods)
            {
                if (!mod.HasDependencies) continue;
                foreach (var dep in mod.dependencies.Select((x) => Dependency.ParseFromString(x)))
                {
                    if (!mods.Any((x) => dep.SatisfiedBy(x)))
                        throw new Exception(
                            $"Unresolved dependency for '{mod.id}'! Cannot find '{dep.mod_id} {dep.mod_version}'");
                }
            }

            return true;
        }

        internal class Dependency
        {
            public string mod_id;
            public SRModInfo.ModVersion mod_version;

            public static Dependency ParseFromString(String s)
            {
                var strings = s.Split(' ');
                var dep = new Dependency
                {
                    mod_id = strings[0],
                    mod_version = SRModInfo.ModVersion.Parse(strings[1])
                };
                return dep;
            }

            public bool SatisfiedBy(SRModLoader.ProtoMod mod)
            {
                return mod.id == mod_id && SRModInfo.ModVersion.Parse(mod.version).CompareTo(mod_version)<=0;
            }
        }
    }
}
