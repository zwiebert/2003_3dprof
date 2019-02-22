#if 0
const char *find_files(const char *dir);
const char *find_dirs(const char *dir);
#endif

class find_files {
  void *data;
public:
  find_files();
  find_files(const char *dir);
  const char *find(const char *dir);
  const char *find();
  const char *get_name();
};

class find_dirs {
  void *data;
public:
  find_dirs();
  find_dirs(const char *dir);
  const char *find(const char *dir);
  const char *find();
  const char *get_name();
};