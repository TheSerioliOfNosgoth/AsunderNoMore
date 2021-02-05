# AsunderNoMore
A command line utility for packing and unpacking Soul Reaver (PC)assets into a new bigfile.dat

Before using:
Place the UNMODIFIED files "bigfile.dat" and "textures.big" in the folder that will become the project. This can be Soul Reaver's install directory if you choose.

To unpack the archive into the repository:
From a command line, type "AsunderNoMore.exe -unpack [FOLDER NAME]" or "AsunderNoMore.exe -u [FOLDER NAME]", where [FOLDER NAME] is the one mentioned above.

Add any new files you've created to the appropriate location in the kain2 or textures folder that has been created.

To pack the repository into a new archive:
From a command line, type "AsunderNoMore.exe -pack [FOLDER NAME]" or "AsunderNoMore.exe -p [FOLDER NAME]", where [FOLDER NAME] is the one mentioned above. Optionally append "-forceAllFiles" or "-f" to force any manually added files to be included in the archive. It is recomended to let Recombobulator automatically mark files for addition instead of using the forceAllFiles option.

The replacement "bigfile.dat" and "textures.big" files can be found in the output folder.