print "if.Win32NT(Win32NT)\n";

for ($i=0x1f, $k=0; $i >= 0; --$i) {
   printf("  name=\"%d\",\t\trv(\"Ogl_95282304\"=hex:%02x,00,00,00),    text=\"%d%%\" \n", $i, $i, -1 * ($i - 100));

}

print "else.Win32NT\n";

for ($i=0x1f, $k=0; $i >= 0; --$i) {
   printf("  name=\"%d\",\t\trv(\"NVidia\\OpenGL\\95282304\"=hex:%02x,00,00,00),    text=\"%d%%\" \n", $i, $i, -1 * ($i - 100));

}

print "endif.Win32NT\n";
