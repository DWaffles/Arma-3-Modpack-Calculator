# Arma-3-Modpack-Calculator
 
This is a simple program to calculate the size of an Arma 3 mod preset, and to provide an automated way to create CSV's for import into other programs. It works by reading all of the mods listed in the HTML preset, then attempts to locate each mod in the Arma workshop directory to calculate it's size on disk. 

The reason it calculates the size on disk rather than using the Steam Workshop given size is that the Workshop size is compressed and can be several Gigabytes off for large mods.