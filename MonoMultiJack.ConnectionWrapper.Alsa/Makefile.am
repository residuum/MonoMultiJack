
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;"
ASSEMBLY = bin/Debug/MonoMultiJack.ConnectionWrapper.Alsa.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll
BUILD_DIR = bin/Debug

MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB_SOURCE=bin/Debug/MonoMultiJack.ConnectionWrapper.Alsa.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Alsa.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/MonoMultiJack.ConnectionWrapper.Alsa.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../MonoMultiJack.ConnectionWrapper/bin/Release/MonoMultiJack.ConnectionWrapper.dll
BUILD_DIR = bin/Release

MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB=
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Release/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB=

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	AssemblyInfo.cs \
	AlsaMidiConnection.cs \
	AlsaMidiManager.cs \
	LibAsoundWrapper.cs \
	LibAsoundWrapper.Definitions.cs \
	LibAsoundWrapper.Pinvoke.cs \
	TypeDefinitions/SndSeqAddr.cs \
	TypeDefinitions/AlsaPort.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	TypeDefinitions \
	monomultijack.connectionwrapper.alsa.pc.in 

REFERENCES =  \
	System \
	Mono.Posix

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

MONOMULTIJACK_CONNECTIONWRAPPER_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_PC = $(BUILD_DIR)/monomultijack.connectionwrapper.alsa.pc

$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB))
$(eval $(call emit-deploy-wrapper,MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_PC,monomultijack.connectionwrapper.alsa.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
