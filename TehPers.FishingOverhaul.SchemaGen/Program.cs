using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema.Generation;
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

            // Create schema generator
            var schemaGenerator = new JsonSchemaGenerator(
                new()
                {
                    ActualSerializerSettings =
                    {
                        Formatting = Formatting.Indented,
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        Converters = new List<JsonConverter> { new StringEnumConverter() },
                    },
                    GenerateExamples = true,
                    FlattenInheritanceHierarchy = true,
                }
            );

            Program.WriteFishingOverhaulSchemas(
                schemaGenerator,
                Path.Join(outDir, "TehPers.FishingOverhaul")
            );
        }

        private static void WriteFishingOverhaulSchemas(
            JsonSchemaGenerator schemaGenerator,
            string outDir
        )
        {
            // Content packs
            Program.WriteSchema<FishTraitsPack>(
                schemaGenerator,
                Path.Join(outDir, "contentPacks/fishTraits.schema.json")
            );
            Program.WriteSchema<FishPack>(
                schemaGenerator,
                Path.Join(outDir, "contentPacks/fish.schema.json")
            );
            Program.WriteSchema<TrashPack>(
                schemaGenerator,
                Path.Join(outDir, "contentPacks/trash.schema.json")
            );
            Program.WriteSchema<TreasurePack>(
                schemaGenerator,
                Path.Join(outDir, "contentPacks/treasure.schema.json")
            );

            // TODO: Configs - program crashes
            // Program.WriteSchema<FishConfig>(
            //     schemaGenerator,
            //     Path.Join(outDir, "configs/fish.schema.json")
            // );
            // Program.WriteSchema<TreasureConfig>(
            //     schemaGenerator,
            //     Path.Join(outDir, "configs/treasure.schema.json")
            // );
            // Program.WriteSchema<HudConfig>(
            //     schemaGenerator,
            //     Path.Join(outDir, "configs/hud.schema.json")
            // );
        }

        private static void WriteSchema<T>(JsonSchemaGenerator schemaGenerator, string path)
        {
            Console.WriteLine($"Generating {path} from {typeof(T).FullName}");
            Program.EnsurePath(path);
            using var outFile = File.Open(path, FileMode.Create);
            using var writer = new StreamWriter(outFile, Encoding.UTF8);
            writer.Write(schemaGenerator.Generate(typeof(T)).ToJson(Formatting.Indented));
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