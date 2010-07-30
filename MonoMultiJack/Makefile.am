
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/MonoMultiJack.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = winexe
PROJECT_REFERENCES =  \
	../Jackd-CIL/bin/Debug/Jackd-CIL.dll
BUILD_DIR = bin/Debug

JACKD_CIL_DLL_SOURCE=../Jackd-CIL/bin/Debug/Jackd-CIL.dll
JACKD_CIL_DLL_MDB_SOURCE=../Jackd-CIL/bin/Debug/Jackd-CIL.dll.mdb
JACKD_CIL_DLL_MDB=$(BUILD_DIR)/Jackd-CIL.dll.mdb
MONOMULTIJACK_EXE_MDB_SOURCE=bin/Debug/MonoMultiJack.exe.mdb
MONOMULTIJACK_EXE_MDB=$(BUILD_DIR)/MonoMultiJack.exe.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/MonoMultiJack.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = winexe
PROJECT_REFERENCES =  \
	../Jackd-CIL/bin/Release/Jackd-CIL.dll
BUILD_DIR = bin/Release

JACKD_CIL_DLL_SOURCE=../Jackd-CIL/bin/Release/Jackd-CIL.dll
JACKD_CIL_DLL_MDB=
MONOMULTIJACK_EXE_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(JACKD_CIL_DLL) \
	$(JACKD_CIL_DLL_MDB) \
	$(MONOMULTIJACK_EXE_MDB)  

BINARIES = \
	$(MONOMULTIJACK)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES = \
	Main.cs \
	AssemblyInfo.cs \
	Forms/MainWindow.cs \
	Configuration/XmlConfiguration.cs \
	Configuration/JackdConfiguration.cs \
	Configuration/AppConfiguration.cs \
	Forms/AppConfigWindow.cs \
	Forms/JackdConfigWindow.cs \
	Widgets/AppWidget.cs \
	Widgets/AppConfigWidget.cs \
	gtk-gui/generated.cs \
	Common/JackdInterop.cs \
	Widgets/JackConnectionsWidget.cs \
	Common/Enums.cs \
	gtk-gui/MonoMultiJack.MainWindow.cs 

DATA_FILES = 

RESOURCES = \
	gtk-gui/gui.stetic 

EXTRAS = \
	Forms \
	Configuration \
	Widgets \
	Common \
	monomultijack.in 

REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS) \
	$(GLADE_SHARP_20_LIBS) \
	Mono.Posix \
	System.Xml \
	System.Core \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

JACKD_CIL_DLL = $(BUILD_DIR)/Jackd-CIL.dll
MONOMULTIJACK = $(BUILD_DIR)/monomultijack

$(eval $(call emit-deploy-target,JACKD_CIL_DLL))
$(eval $(call emit-deploy-target,JACKD_CIL_DLL_MDB))
$(eval $(call emit-deploy-wrapper,MONOMULTIJACK,monomultijack,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
