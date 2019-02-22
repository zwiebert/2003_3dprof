#include <stdlib.h>
#include <stdio.h>

#define TEST_TIMEOUT 1
#define SLEEP_FOREVER 0

#if TEST_TIMEOUT
// ???-bw: The timer could be in this program or the parent process could kill us (less time() calls).
#include <time.h>
#include <stdio.h>
#include <sys/types.h>
#include <sys/timeb.h>
#include <string.h>
#define TIMEOUT_MAX_SECS 30
#endif /* TEST_TIMEOUT */

int alloc_size = 256 * 1024 * 1024;
int page_size = 4096;

void touch_mem(char *mem)
{
  int i;
  time_t s_time;

  time(&s_time);

  for (i=0; i < alloc_size; i+= page_size) {
    mem[i] = ~0;
#if TEST_TIMEOUT
    if ((i % (4 * 1024 * 1024)) == 0) {
      if ((time(0) - s_time) >= TIMEOUT_MAX_SECS)
        return;
    }
#endif /* TEST_TIMEOUT */
  }
}

int main(int ac, char **av)
{
  int i;
  int opt_wait = 0;
  char *mem;
#if SLEEP_FOREVER
  while(1)
   _sleep(1000);
#endif

  puts("page_out_ram [wait] [size to alloc in MB]\n\n"
    "Examples:\n"
    "page_ram_out.exe\n"
    "page_ram_out.exe wait\n"
    "page_ram_out.exe 256\n"
    "page_ram_out.exe wait 384\n");

  for (i=1; i < ac; ++i) {
    if (strcmp (av [i], "wait") == 0)
      opt_wait = 1;
    else {
      alloc_size = atoi(av[i]) * 1024 * 1024;
    }
  }


  mem = malloc(alloc_size);
  if (!mem)
    return 1;

  touch_mem(mem);

  if (opt_wait) {
    fputs("Press return to continue\n", stderr);
    getchar();
  }

  free(mem);
  return 0;
}
