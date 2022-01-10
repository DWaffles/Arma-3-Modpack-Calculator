# Arma-3-Modpack-Calculator

This simple programs aims to provide basic information, such as the size and dependencies, of an Arma 3 mod preset and provide an automated way to export that data. It works by reading all of the mods, and their metadata, listed in the HTML preset and attempting to locate each mod in the Arma workshop directory to calculate it's size on disk. 

The reason it calculates the size on disk rather than using the Steam Workshop given size is that the Workshop size is compressed and can be several gigabytes off for large mods. It does however use the Steam Workshop page to calculate dependencies.

While unfinished with user interface, the core logic is mostly completed with only minor issues.

# Built With

- [AngleSharp](https://anglesharp.github.io/)
- [ByteSize](https://github.com/omar/ByteSize)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)