#include <iostream>
#include <string>
#include <cstring>
#include <iomanip>
#include <tchar.h>
#include <windows.h>
#include <strstream>

#include "nvidia_hardware.h"
#include "nvidia_regs.h"

using namespace std;


string current_time_stamp()
{
  // Declare a 'SYSTEMTIME' structure
  SYSTEMTIME d;

  // Get the current date
  GetLocalTime(&d);

  // Buffer to hold formatted date
  char buffer[64];
  strstream outDate(buffer, sizeof buffer, ios::out);

  // Format the date how you want it
  outDate.fill('0');
  // NOTE: Casting should not be needed
  outDate
    << setw(2) << d.wHour << ":"
    << setw(2) << d.wMinute << ":"
    << setw(2) << d.wSecond << ","
    << setw(3) << d.wMilliseconds
    << ends;

  // Formatted date is now in 'buffer'. You can now print it etc.
  // The following just prints it
  return outDate.str();
}



void dump(void *base, unsigned size)
{
  unsigned *mem = (unsigned *)base;
  unsigned col_size = 4;
  unsigned dumped_bytes = 0;

  cout.fill('0');

  for (unsigned lin=0; dumped_bytes < size; ++lin) {
    cout << std::nouppercase << std::hex << setw(4) << dumped_bytes << ": ";
    for (unsigned col=0; col < col_size && dumped_bytes < size; ++col, (dumped_bytes+=4)) {
      cout << ((col > 0) ? "    " : "")
        << std::nouppercase << std::hex << setw(8)
        << ((unsigned *)base)[lin * col_size + col];
    }
    cout << endl;
  }

}

bool opt_log_gpu_clk, opt_log_mem_clk, opt_dump_registers;
unsigned long optarg_log_interval = 500;
int mem_type = 1; /* Normal=1, DDR=2 */

void print_usage(const char *progname) 
{
  char *s = strrchr(progname, '\\');
  if (s)
    progname = s+1;

  cerr << "Usage: " << progname << " options"
    << endl
    << " --log-gpu-clk\t\t Enable logging of GPU clock" << endl
    << " --log-mem-clk\t\t Enable logging of RAM clock (real clock, no DDR)" << endl
    << " --log-interval N\t Time in ms between polling clock [500]" << endl
    << " --dump-registers\t Hexdump of chip registers" << endl
    << endl;
}

int _tmain(int argc, _TCHAR* argv[])
{
  if (argc == 1) {
    print_usage(argv[0]);
    opt_log_gpu_clk = true;
    opt_log_mem_clk = true;
  } else for (int i=1; i < argc; ++i) {
    if (0==stricmp(argv[i], "--log-gpu-clk"))
      opt_log_gpu_clk = true;
    else if (0==stricmp(argv[i], "--log-mem-clk"))
      opt_log_mem_clk = true;
    else if (0==stricmp(argv[i], "--log-interval"))
      optarg_log_interval = strtoul(argv[++i], 0, 10);
    else if (0==stricmp(argv[i], "--dump-registers"))
      opt_dump_registers = true;

  }


    if (nvc_init()) {
      nvc_set_card(0);

      mem_type = ((strcmp("DDR", nvc_get_memory_type()) == 0) ? 2 : 1);

      //std::cout << "Clocks " << nvc_get_gpu_speed() <<  "/" << nvc_get_memory_speed() / 2 << " (core/mem)" << std::endl;
      std::cout << "GPU Speed: " << nvc_get_gpu_speed() << std::endl;
      std::cout << "Mem Speed: " << (nvc_get_memory_speed() / mem_type) << (mem_type == 2 ? " DDR" : " SDR") << std::endl;
      std::cout << "Mem Size: " << nvc_get_memory_size() << std::endl;
      std::cout << "Fast Write: " << nvc_get_fw_status() << std::endl;
      std::cout << "AGP Rate: " << nvc_get_agp_rate() << std::endl;
      std::cout << "Mem Type: " << nvc_get_memory_type() << std::endl;
      std::cout << "SBA Status: " << nvc_get_sba_status() << std::endl;
      cout << endl;

      if (opt_dump_registers) {
        cout << endl << "######### Dump MC ###########" << endl;
        dump(nvc_get_regspace_mc, NVIDIA_MC_BYTE_SIZE);
        cout << "######### Dump ExtDev ##########" << endl;
        dump(nvc_get_regspace_extdev, NVIDIA_EXTDEV_BYTE_SIZE);
        cout << endl << "######### Dump FB ###########" << endl;
        dump(nvc_get_regspace_fb, NVIDIA_FB_BYTE_SIZE);
        cout << endl << "######### Dump RAMDAC ###########" << endl;
        dump(nvc_get_regspace_ramdac, NVIDIA_RAMDAC_BYTE_SIZE);
      }



      if (opt_log_gpu_clk || opt_log_mem_clk) {
        if (optarg_log_interval < 5) {
          SetPriorityClass(GetCurrentProcess(), REALTIME_PRIORITY_CLASS);
          cerr
            << "====> Priority Class changed to Realtime! If this causes trouble, change" << endl
            << "====> it back with taskmanager." << endl;
        } else if (optarg_log_interval < 200) {
          SetPriorityClass(GetCurrentProcess(), HIGH_PRIORITY_CLASS);
          cerr << "====> Priority Class changed to High!" << endl;
        } else if (optarg_log_interval < 750) {
          SetPriorityClass(GetCurrentProcess(), ABOVE_NORMAL_PRIORITY_CLASS);
          cerr << "====> Priority Class changed to Above Normal!" << endl;
        }



        cerr 
          << "====> Press Ctrl-C to end the program" << endl
          << "====> Use \"> filename\" in commandline to redirect output"
          << endl << endl;

        float old_nvclk = 0.0, old_memclk = 0.0;
        cout.setf(std::ios::fixed, std::ios::floatfield);
        cout << std::setprecision(3);

        while (Sleep(optarg_log_interval), true) {
          float nvclk = nvc_get_gpu_speed();
          float memclk = nvc_get_memory_speed() / mem_type;

          if ((old_nvclk == nvclk) && (old_memclk == memclk))
            continue;

          if (false && optarg_log_interval >= 500) {
            if ((old_nvclk != nvclk) || (old_memclk != memclk))
              cout << "* " << current_time_stamp() << endl;

            if (old_nvclk != nvclk) {
              cout << "** GPU clock change: " << old_nvclk << " MHz => " << nvclk << " MHz" << endl; 
              old_nvclk = nvclk;
            }
            if (old_memclk != memclk) {
              cout << "** RAM clock change: " << old_memclk << " MHz => " << memclk << " MHz" << endl; 
              old_memclk = memclk;
            }

          } else {
            cout << "* " << current_time_stamp() << ": ";
            if (opt_log_gpu_clk)
              cout << "GPU = " << nvclk << " MHz   ";
            if (opt_log_mem_clk)
              cout << "RAM = " << memclk << " MHz";
            cout << endl; 
            old_nvclk = nvclk;
            old_memclk = memclk;
          }
        }
      }
      nvc_leave();
    } else {
      cerr << "Error: Cannot access card\n" << endl;
    }
    return 0;
}

