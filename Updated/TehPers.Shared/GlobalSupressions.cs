using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633", Justification = "Open source mods don't need a header on every file.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "CA1815", Justification = "Not all structs should override equality operators.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1407", Justification = "Some operator precedence is obvious.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "CA2225", Justification = "Extra static members as friendly names for operators are redundant.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "CA1303", Justification = "I don't have a localization team.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "CA2208", Justification = "I don't have a localization team.")]
[assembly: SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "I'll call my types whatever I feel is appropriate. There are many cases where a collection type doesn't end in the word 'Collection', for example.")]
