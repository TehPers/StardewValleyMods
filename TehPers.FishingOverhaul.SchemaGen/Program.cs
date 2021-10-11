using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Config.ContentPacks;

namespace TehPers.FishingOverhaul.SchemaGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Check args
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: <outDir>");
                return;
            }

            // Get output directory
            var outDir = Path.GetFullPath(args[0]);
            Console.WriteLine($"Output directory: {outDir}");

            // Generate the schemas
            Program.WriteFishingOverhaulSchemas(
                Path.Join(outDir, "TehPers.FishingOverhaul", "schemas")
            );
        }

        private static void WriteFishingOverhaulSchemas(string outDir)
        {
            // Content packs
            Program.WriteSchema<FishTraitsPack>(
                Path.Join(outDir, "contentPacks/fishTraits.schema.json")
            );
            Program.WriteSchema<FishPack>(Path.Join(outDir, "contentPacks/fish.schema.json"));
            Program.WriteSchema<TrashPack>(Path.Join(outDir, "contentPacks/trash.schema.json"));
            Program.WriteSchema<TreasurePack>(
                Path.Join(outDir, "contentPacks/treasure.schema.json")
            );

            // Configs
            Program.WriteSchema<FishConfig>(Path.Join(outDir, "configs/fish.schema.json"));
            Program.WriteSchema<TreasureConfig>(Path.Join(outDir, "configs/treasure.schema.json"));
            Program.WriteSchema<HudConfig>(Path.Join(outDir, "configs/hud.schema.json"));
        }

        private static void WriteSchema<T>(string path)
        {
            Console.WriteLine($"Generating {path} from {typeof(T).FullName}");
            Program.EnsurePath(path);
            using var outFile = File.Open(path, FileMode.Create);
            using var writer = new StreamWriter(outFile, Encoding.UTF8);

            // Generate schema
            var definitionMap = new DefinitionMap();
            var schema = definitionMap.Register(typeof(FishPack).ToContextualType());

            // Add standard properties
            schema["$schema"] = "http://json-schema.org/draft-04/schema#";
            schema["title"] = nameof(FishPack);

            // Add definitions
            var definitions = new JObject();
            schema["definitions"] = definitions;
            foreach (var (name, defSchema) in definitionMap.Definitions)
            {
                definitions[name] = defSchema;
            }

            // Write schema
            writer.Write(schema.ToString(Formatting.Indented));
        }

        private static void EnsurePath(string path)
        {
            if (Path.GetDirectoryName(path) is { } dir)
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}