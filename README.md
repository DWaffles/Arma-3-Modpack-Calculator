# Arma-3-Modpack-Calculator

This simple program aims to provide basic information, such as the size and dependencies, of an Arma 3 mod preset and provide an automated way to export that data for use in other programs.

It works by reading all of the mods listed in the HTML preset and attempts to locate each mod in the Arma workshop directory to calculate it's size on disk. This is because the size on the Steam Workshop is compressed and can lead to differences of several gigabytes. Optionally, a second preset can be designated as the previous preset, and the program will calculate which mods were added and which were removed between the two mod sets.

While the calculator overall is still a work in progress, the core logic is mostly completed.

# Built With

- [AngleSharp](https://anglesharp.github.io/)
- [ByteSize](https://github.com/omar/ByteSize)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [Spectre.Console](https://spectreconsole.net/)
- [Stack Overflow](https://stackoverflow.com/)