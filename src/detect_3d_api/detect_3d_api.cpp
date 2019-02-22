#include <iostream>
#include <fstream>
#include <algorithm>
#include <stack>
#include <string>

#include "find_files.h"

using namespace std;


struct pattern {
  size_t pat_len;
  char *pat, *pat_lc, *pat_uc;
  unsigned idx;
  int found;
  int skip; // not used

  pattern(const char *s)
    : pat_len(strlen(s))
    , pat(strcpy((char*)malloc(pat_len + 1), s))
    , pat_lc(strcpy((char*)malloc(pat_len + 1), s))
    , pat_uc(strcpy((char*)malloc(pat_len + 1), s))
    , idx(0)
    , found(0)
    , skip(0)
  {
    for (char *p = pat_lc; *p; ++p)
      *p = tolower(*p);
    for (char *p = pat_uc; *p; ++p)
      *p = toupper(*p);
  }
  ~pattern() {
    free(pat), free(pat_lc), free(pat_uc);
  }

  bool push_byte(char c) {
    if (pat_lc[idx] == c || pat_uc[idx] == c) {
      if (++idx >= pat_len) {
        ++found;
        idx = 0;
        return true;
      }
    } else {
      idx = 0;
    }
    return false;
  }

  int nmb_matches() {
    return found;
  }

};

#define TEST_DIR  "D:/sierra/half-life"
//#define TEST_FILE  "test_text.txt"
#define BUF_SIZE (2 * 1024 * 1024)  // filebuf buffer size


static pattern pat_d3d("d3d"), pat_opengl32("OpenGL32");
static filebuf fb;
static char file_name_buf[255];


void do_file(const char *file_name) {

  fb.open (file_name, (ios::in | ios::binary));
  istream is(&fb);

  char c;
  while (is >> c) {
    pat_d3d.push_byte(c);
    pat_opengl32.push_byte(c);
  }

  fb.close();

}



void do_directory(const char *dir_name_a) {

  find_files *ffs = new find_files();
  find_dirs *fds = new find_dirs();
  char *dir_name = strcpy(new char[strlen(dir_name_a) + 1], dir_name_a);

  for (const char *sub_dir_name = fds->find(strcat(strcpy(file_name_buf, dir_name), "/*.*"));
    sub_dir_name; sub_dir_name = fds->find())
    if (strcmp(".", sub_dir_name) == 0 || strcmp("..", sub_dir_name) == 0)
      continue;
    else
      do_directory(strcat(strcat(strcpy(file_name_buf, dir_name), "/"), sub_dir_name));


  for (const char *file_name = ffs->find(strcat(strcat(strcpy(file_name_buf, dir_name), "/"), "*.exe"));
    file_name; file_name = ffs->find())
    do_file(strcat(strcat(strcpy(file_name_buf, dir_name), "/"), file_name));

  for (const char *file_name = ffs->find(strcat(strcat(strcpy(file_name_buf, dir_name), "/"), "*.dll"));
    file_name; file_name = ffs->find())
    do_file(strcat(strcat(strcpy(file_name_buf, dir_name), "/"), file_name));

  delete dir_name;
}



int main(int ac, char **av)
{
  char *buf = new char[BUF_SIZE];
  fb.pubsetbuf(buf, BUF_SIZE);
  char *suffixes[] = {"*.dll", "*.exe", 0};

  if (ac != 2)
    return 1;

  const char *dir = (ac == 2) ? av[1] : TEST_DIR;

  do_directory(dir);

  if (pat_d3d.nmb_matches())
    cout << "d3d=" << pat_d3d.nmb_matches();
  else
    cout << "-";
  cout << " / ";

  if (pat_opengl32.nmb_matches())
    cout << "ogl=" << pat_opengl32.nmb_matches();
  else
    cout << "-";

  cout  << endl;

}



