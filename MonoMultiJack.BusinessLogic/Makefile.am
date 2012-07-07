
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/MonoMultiJack.BusinessLogic.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll \
	../MonoMultiJack.ConnectionWrapper.Alsa/bin/Debug/MonoMultiJack.ConnectionWrapper.Alsa.dll \
	../MonoMultiJack.ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll
BUILD_DIR = bin/Debug

MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB_SOURCE=bin/Debug/MonoMultiJack.BusinessLogic.dll.mdb
MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.BusinessLogic.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Debug/MonoMultiJack.ConnectionWrapper.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper.Alsa/bin/Debug/MonoMultiJack.ConnectionWrapper.Alsa.dll
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB_SOURCE=../MonoMultiJack.ConnectionWrapper.Alsa/bin/Debug/MonoMultiJack.ConnectionWrapper.Alsa.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Alsa.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/bin/Debug/MonoMultiJack.ConnectionWrapper.Jack.dll.mdb
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB=$(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Jack.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/MonoMultiJack.BusinessLogic.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../MonoMultiJack.ConnectionWrapper/bin/Release/MonoMultiJack.ConnectionWrapper.dll \
	../MonoMultiJack.ConnectionWrapper.Alsa/bin/Release/MonoMultiJack.ConnectionWrapper.Alsa.dll \
	../MonoMultiJack.ConnectionWrapper.Jack/bin/Release/MonoMultiJack.ConnectionWrapper.Jack.dll
BUILD_DIR = bin/Release

MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB=
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper/bin/Release/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB=
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper.Alsa/bin/Release/MonoMultiJack.ConnectionWrapper.Alsa.dll
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB=
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_SOURCE=../MonoMultiJack.ConnectionWrapper.Jack/bin/Release/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB=

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(MONOMULTIJACK_BUSINESSLOGIC_DLL_MDB) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL) \
	$(MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(MONOMULTIJACK_BUSINESSLOGIC_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	AssemblyInfo.cs \
	Common/ProgramManagement.cs \
	Configuration/AppConfiguration.cs \
	Configuration/JackdConfiguration.cs \
	Configuration/PersistantConfiguration.cs \
	Common/ConnectionManagerFactory.cs \
	Configuration/WindowConfiguration.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	monomultijack.businesslogic.pc.in 

REFERENCES =  \
	System \
	System.Xml \
	$(GLIB_SHARP_20_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

MONOMULTIJACK_CONNECTIONWRAPPER_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.dll
MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Alsa.dll
MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL = $(BUILD_DIR)/MonoMultiJack.ConnectionWrapper.Jack.dll
MONOMULTIJACK_BUSINESSLOGIC_PC = $(BUILD_DIR)/monomultijack.businesslogic.pc

$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_DLL_MDB))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_ALSA_DLL_MDB))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL))
$(eval $(call emit-deploy-target,MONOMULTIJACK_CONNECTIONWRAPPER_JACK_DLL_MDB))
$(eval $(call emit-deploy-wrapper,MONOMULTIJACK_BUSINESSLOGIC_PC,monomultijack.businesslogic.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
