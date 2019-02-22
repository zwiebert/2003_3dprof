#! /bin/sh

function extract_cs() {
  make_var=src_${1}
  make_var_tgt=bin_${1}
  path=$2
  project_file=$3
  target=${4}
  project_name=$(basename $project_file .csproj)
cat <<EOF
${make_var} = \$(addprefix $path/, \\
 $project_file \\
EOF
  grep RelPath $path/$project_file |
    sed -e 's/^.*RelPath = \"\(.*\)\".*$/\1/' \
        -e 's=\\=\/=g' -e 's/$/ \\/' -e 's=^\( *\)\./=\1='
cat <<EOF
 )

${make_var_tgt} = ${target}

sources        += \$(${make_var}) 
binaries       += \$(${make_var_tgt}) 

### Building ${target}
${make_var_tgt} \$(${make_var_tgt}) : \$(${make_var})
	\$(devenv) r6clock.sln /build Release /project ${project_name}

EOF
}


function extract_vc() {
  make_var=src_${1}
  make_var_tgt=bin_${1}
  path=$2
  project_file=$3
  target=${4}
  project_deps=${5}
  project_name=$(grep 'Name=' $path/$project_file | head -1 | sed -e 's/.*Name="\(.*\)".*$/\1/')
cat <<EOF
${make_var} = \$(addprefix $path/, \\
 $project_file \\
EOF
  grep RelativePath $path/$project_file |
    sed -e 's/^.*RelativePath=\"\(.*\)\".*$/ \1/' \
        -e 's=\\=\/=g' -e 's/$/ \\/' -e 's=^\( *\)\./=\1='
cat <<EOF
 )

${make_var_tgt} = ${target}

sources        += \$(${make_var}) 
binaries       += \$(${make_var_tgt}) 

EOF

cat <<EOF >>temp_rules.txt
### Building ${target}
${make_var_tgt} \$(${make_var_tgt}) : \$(${make_var}) ${project_deps}
	\$(devenv) r6clock.sln /build Release /project ${project_name}

EOF
}

function extract_vd() {
  make_var=src_${1}
  make_var_tgt=bin_${1}
  path=$2
  project_file=$3
  target=${4}
  project_deps=${5}
  project_name=$(grep '"ProjectName" =' $path/$project_file | head -1 | sed -e 's/.*"ProjectName" = "8:\(.*\)".*$/\1/')
cat <<EOF
${make_var} = \$(addprefix $path/, \\
 $project_file \\
EOF
  grep RelativePath $path/$project_file |
    sed -e 's/^.*RelativePath=\"\(.*\)\".*$/ \1/' \
        -e 's=\\=\/=g' -e 's/$/ \\/' -e 's=^\( *\)\./=\1='
cat <<EOF
 )

${make_var_tgt} = ${target}

sources        += \$(${make_var}) 
binaries       += \$(${make_var_tgt}) 

EOF

cat <<EOF >>temp_rules.txt
### Building ${target}
${make_var_tgt} \$(${make_var_tgt}) : \$(${make_var}) ${project_deps}
	\$(devenv) r6clock.sln /build Release /project ${project_name}

EOF
}


rm -f temp_rules.txt

cat <<EOF
################################################################################
### Do not edit! This file was automtically generated by make_deps.sh ##########
################################################################################

EOF

extract_vc r6_sys     .           r6probe-sys.vcproj     r6probe.sys
extract_vc r6_vxd     .           r6probe-vxd.vcproj     r6probe.vxd
extract_vc r6_lib     r6probe     r6probe-lib.vcproj     r6probe/Release/r6probe.lib
extract_vc r6clk_lib  setclk      setclk-lib.vcproj      setclk/Release/setclk.lib
extract_vc r6clk_exe  setclk_exe  setclk.vcproj          setclk_exe/Release/radeon_setclk.exe '$(bin_r6_lib) $(bin_r6clk_lib)'
extract_vc r6clock-dll r6clock-dll r6clock-dll.vcproj    r6clock-dll/Release/r6clock.dll  '$(bin_r6_lib) $(bin_r6clk_lib)'
extract_vc clocker    clocker     clocker.vcproj         clocker/Release/clocker.exe     '$(bin_r6clock-dll)'

extract_vd r6clock-dll-bin r6clock-dll-bin r6clock-dll-bin.vdproj   r6clock-dll-bin/Release/r6clock-dll-bin.CAB  '$(bin_r6_lib) $(bin_r6clk_lib)'


extract_vc r6clock-dll-example-C  r6clock-dll/examples/C r6clock-dll-example-C.vcproj   r6clock-dll/examples/C/Release/r6clock-dll-example-C.exe  '$(bin_r6clock-dll)'


cat temp_rules.txt

