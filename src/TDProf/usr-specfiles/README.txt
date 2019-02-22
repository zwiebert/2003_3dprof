This directory holds files created by user to ovveride the ones in
../specfiles directory.

For syntax see ../specfiles/README.txt

If you want to extent other files you should use a include command in your
file. Please note, that paths are relative to application
directory (..).

To include ../specfiles/nvidia-common-win5.cfg so you have to write:

include="specfiles/nvidia-common-win5.cfg"

To include a file from this directory you have to write:

include="usr-specfiles/some_user_file.cfg"

Note that you can define a label as often as you like. Only items
defined by the label parsed at last will used.

