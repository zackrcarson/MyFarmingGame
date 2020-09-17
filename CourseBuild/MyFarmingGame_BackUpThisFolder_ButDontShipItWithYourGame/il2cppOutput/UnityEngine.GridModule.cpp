#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <cstring>
#include <string.h>
#include <stdio.h>
#include <cmath>
#include <limits>
#include <assert.h>
#include <stdint.h>

#include "codegen/il2cpp-codegen.h"
#include "il2cpp-object-internals.h"


// System.String
struct String_t;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;
// UnityEngine.Grid
struct Grid_t9989A4B9E81E8D350DF09DE256FF140F22AB6476;
// UnityEngine.GridLayout
struct GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9;

IL2CPP_EXTERN_C RuntimeClass* Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C const uint32_t Grid_GetCellCenterWorld_mE691618405B70E63E29EE21E5512338701C6A4E5_MetadataUsageId;


IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_t4C4EBABD90160E739081DCA11D3BD07EAD12BC83 
{
public:

public:
};


// System.Object

struct Il2CppArrayBounds;

// System.Array


// System.ValueType
struct  ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF  : public RuntimeObject
{
public:

public:
};

// Native definition for P/Invoke marshalling of System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF_marshaled_com
{
};

// System.IntPtr
struct  IntPtr_t 
{
public:
	// System.Void* System.IntPtr::m_value
	void* ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(IntPtr_t, ___m_value_0)); }
	inline void* get_m_value_0() const { return ___m_value_0; }
	inline void** get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(void* value)
	{
		___m_value_0 = value;
	}
};

struct IntPtr_t_StaticFields
{
public:
	// System.IntPtr System.IntPtr::Zero
	intptr_t ___Zero_1;

public:
	inline static int32_t get_offset_of_Zero_1() { return static_cast<int32_t>(offsetof(IntPtr_t_StaticFields, ___Zero_1)); }
	inline intptr_t get_Zero_1() const { return ___Zero_1; }
	inline intptr_t* get_address_of_Zero_1() { return &___Zero_1; }
	inline void set_Zero_1(intptr_t value)
	{
		___Zero_1 = value;
	}
};


// System.Void
struct  Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017 
{
public:
	union
	{
		struct
		{
		};
		uint8_t Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017__padding[1];
	};

public:
};


// UnityEngine.Vector3
struct  Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 
{
public:
	// System.Single UnityEngine.Vector3::x
	float ___x_2;
	// System.Single UnityEngine.Vector3::y
	float ___y_3;
	// System.Single UnityEngine.Vector3::z
	float ___z_4;

public:
	inline static int32_t get_offset_of_x_2() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___x_2)); }
	inline float get_x_2() const { return ___x_2; }
	inline float* get_address_of_x_2() { return &___x_2; }
	inline void set_x_2(float value)
	{
		___x_2 = value;
	}

	inline static int32_t get_offset_of_y_3() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___y_3)); }
	inline float get_y_3() const { return ___y_3; }
	inline float* get_address_of_y_3() { return &___y_3; }
	inline void set_y_3(float value)
	{
		___y_3 = value;
	}

	inline static int32_t get_offset_of_z_4() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___z_4)); }
	inline float get_z_4() const { return ___z_4; }
	inline float* get_address_of_z_4() { return &___z_4; }
	inline void set_z_4(float value)
	{
		___z_4 = value;
	}
};

struct Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields
{
public:
	// UnityEngine.Vector3 UnityEngine.Vector3::zeroVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___zeroVector_5;
	// UnityEngine.Vector3 UnityEngine.Vector3::oneVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___oneVector_6;
	// UnityEngine.Vector3 UnityEngine.Vector3::upVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___upVector_7;
	// UnityEngine.Vector3 UnityEngine.Vector3::downVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___downVector_8;
	// UnityEngine.Vector3 UnityEngine.Vector3::leftVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___leftVector_9;
	// UnityEngine.Vector3 UnityEngine.Vector3::rightVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___rightVector_10;
	// UnityEngine.Vector3 UnityEngine.Vector3::forwardVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___forwardVector_11;
	// UnityEngine.Vector3 UnityEngine.Vector3::backVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___backVector_12;
	// UnityEngine.Vector3 UnityEngine.Vector3::positiveInfinityVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___positiveInfinityVector_13;
	// UnityEngine.Vector3 UnityEngine.Vector3::negativeInfinityVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___negativeInfinityVector_14;

public:
	inline static int32_t get_offset_of_zeroVector_5() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___zeroVector_5)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_zeroVector_5() const { return ___zeroVector_5; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_zeroVector_5() { return &___zeroVector_5; }
	inline void set_zeroVector_5(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___zeroVector_5 = value;
	}

	inline static int32_t get_offset_of_oneVector_6() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___oneVector_6)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_oneVector_6() const { return ___oneVector_6; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_oneVector_6() { return &___oneVector_6; }
	inline void set_oneVector_6(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___oneVector_6 = value;
	}

	inline static int32_t get_offset_of_upVector_7() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___upVector_7)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_upVector_7() const { return ___upVector_7; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_upVector_7() { return &___upVector_7; }
	inline void set_upVector_7(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___upVector_7 = value;
	}

	inline static int32_t get_offset_of_downVector_8() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___downVector_8)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_downVector_8() const { return ___downVector_8; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_downVector_8() { return &___downVector_8; }
	inline void set_downVector_8(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___downVector_8 = value;
	}

	inline static int32_t get_offset_of_leftVector_9() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___leftVector_9)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_leftVector_9() const { return ___leftVector_9; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_leftVector_9() { return &___leftVector_9; }
	inline void set_leftVector_9(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___leftVector_9 = value;
	}

	inline static int32_t get_offset_of_rightVector_10() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___rightVector_10)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_rightVector_10() const { return ___rightVector_10; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_rightVector_10() { return &___rightVector_10; }
	inline void set_rightVector_10(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___rightVector_10 = value;
	}

	inline static int32_t get_offset_of_forwardVector_11() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___forwardVector_11)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_forwardVector_11() const { return ___forwardVector_11; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_forwardVector_11() { return &___forwardVector_11; }
	inline void set_forwardVector_11(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___forwardVector_11 = value;
	}

	inline static int32_t get_offset_of_backVector_12() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___backVector_12)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_backVector_12() const { return ___backVector_12; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_backVector_12() { return &___backVector_12; }
	inline void set_backVector_12(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___backVector_12 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_13() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___positiveInfinityVector_13)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_positiveInfinityVector_13() const { return ___positiveInfinityVector_13; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_positiveInfinityVector_13() { return &___positiveInfinityVector_13; }
	inline void set_positiveInfinityVector_13(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___positiveInfinityVector_13 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_14() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___negativeInfinityVector_14)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_negativeInfinityVector_14() const { return ___negativeInfinityVector_14; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_negativeInfinityVector_14() { return &___negativeInfinityVector_14; }
	inline void set_negativeInfinityVector_14(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___negativeInfinityVector_14 = value;
	}
};


// UnityEngine.Vector3Int
struct  Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 
{
public:
	// System.Int32 UnityEngine.Vector3Int::m_X
	int32_t ___m_X_0;
	// System.Int32 UnityEngine.Vector3Int::m_Y
	int32_t ___m_Y_1;
	// System.Int32 UnityEngine.Vector3Int::m_Z
	int32_t ___m_Z_2;

public:
	inline static int32_t get_offset_of_m_X_0() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4, ___m_X_0)); }
	inline int32_t get_m_X_0() const { return ___m_X_0; }
	inline int32_t* get_address_of_m_X_0() { return &___m_X_0; }
	inline void set_m_X_0(int32_t value)
	{
		___m_X_0 = value;
	}

	inline static int32_t get_offset_of_m_Y_1() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4, ___m_Y_1)); }
	inline int32_t get_m_Y_1() const { return ___m_Y_1; }
	inline int32_t* get_address_of_m_Y_1() { return &___m_Y_1; }
	inline void set_m_Y_1(int32_t value)
	{
		___m_Y_1 = value;
	}

	inline static int32_t get_offset_of_m_Z_2() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4, ___m_Z_2)); }
	inline int32_t get_m_Z_2() const { return ___m_Z_2; }
	inline int32_t* get_address_of_m_Z_2() { return &___m_Z_2; }
	inline void set_m_Z_2(int32_t value)
	{
		___m_Z_2 = value;
	}
};

struct Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields
{
public:
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_Zero
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_Zero_3;
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_One
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_One_4;
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_Up
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_Up_5;
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_Down
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_Down_6;
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_Left
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_Left_7;
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_Right
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_Right_8;

public:
	inline static int32_t get_offset_of_s_Zero_3() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_Zero_3)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_Zero_3() const { return ___s_Zero_3; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_Zero_3() { return &___s_Zero_3; }
	inline void set_s_Zero_3(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_Zero_3 = value;
	}

	inline static int32_t get_offset_of_s_One_4() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_One_4)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_One_4() const { return ___s_One_4; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_One_4() { return &___s_One_4; }
	inline void set_s_One_4(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_One_4 = value;
	}

	inline static int32_t get_offset_of_s_Up_5() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_Up_5)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_Up_5() const { return ___s_Up_5; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_Up_5() { return &___s_Up_5; }
	inline void set_s_Up_5(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_Up_5 = value;
	}

	inline static int32_t get_offset_of_s_Down_6() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_Down_6)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_Down_6() const { return ___s_Down_6; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_Down_6() { return &___s_Down_6; }
	inline void set_s_Down_6(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_Down_6 = value;
	}

	inline static int32_t get_offset_of_s_Left_7() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_Left_7)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_Left_7() const { return ___s_Left_7; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_Left_7() { return &___s_Left_7; }
	inline void set_s_Left_7(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_Left_7 = value;
	}

	inline static int32_t get_offset_of_s_Right_8() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_Right_8)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_Right_8() const { return ___s_Right_8; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_Right_8() { return &___s_Right_8; }
	inline void set_s_Right_8(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_Right_8 = value;
	}
};


// UnityEngine.Object
struct  Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0  : public RuntimeObject
{
public:
	// System.IntPtr UnityEngine.Object::m_CachedPtr
	intptr_t ___m_CachedPtr_0;

public:
	inline static int32_t get_offset_of_m_CachedPtr_0() { return static_cast<int32_t>(offsetof(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0, ___m_CachedPtr_0)); }
	inline intptr_t get_m_CachedPtr_0() const { return ___m_CachedPtr_0; }
	inline intptr_t* get_address_of_m_CachedPtr_0() { return &___m_CachedPtr_0; }
	inline void set_m_CachedPtr_0(intptr_t value)
	{
		___m_CachedPtr_0 = value;
	}
};

struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_StaticFields
{
public:
	// System.Int32 UnityEngine.Object::OffsetOfInstanceIDInCPlusPlusObject
	int32_t ___OffsetOfInstanceIDInCPlusPlusObject_1;

public:
	inline static int32_t get_offset_of_OffsetOfInstanceIDInCPlusPlusObject_1() { return static_cast<int32_t>(offsetof(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_StaticFields, ___OffsetOfInstanceIDInCPlusPlusObject_1)); }
	inline int32_t get_OffsetOfInstanceIDInCPlusPlusObject_1() const { return ___OffsetOfInstanceIDInCPlusPlusObject_1; }
	inline int32_t* get_address_of_OffsetOfInstanceIDInCPlusPlusObject_1() { return &___OffsetOfInstanceIDInCPlusPlusObject_1; }
	inline void set_OffsetOfInstanceIDInCPlusPlusObject_1(int32_t value)
	{
		___OffsetOfInstanceIDInCPlusPlusObject_1 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_marshaled_pinvoke
{
	intptr_t ___m_CachedPtr_0;
};
// Native definition for COM marshalling of UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_marshaled_com
{
	intptr_t ___m_CachedPtr_0;
};

// UnityEngine.Component
struct  Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:

public:
};


// UnityEngine.Behaviour
struct  Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8  : public Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621
{
public:

public:
};


// UnityEngine.GridLayout
struct  GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9  : public Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8
{
public:

public:
};


// UnityEngine.Grid
struct  Grid_t9989A4B9E81E8D350DF09DE256FF140F22AB6476  : public GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif



// UnityEngine.Vector3 UnityEngine.Vector3Int::op_Implicit(UnityEngine.Vector3Int)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3Int_op_Implicit_m8403835CB506B9EAA8510C113EDB537455F05CC3 (Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___v0, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.GridLayout::GetLayoutCellCenter()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  GridLayout_GetLayoutCellCenter_m0012793E35AF6CAA364242BF669F9A908E5F8ADD (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::op_Addition(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_op_Addition_m929F9C17E5D11B94D50B4AFF1D730B70CB59B50E (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___a0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___b1, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.GridLayout::CellToLocalInterpolated(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  GridLayout_CellToLocalInterpolated_m7F44E7F9CEE03E76CD5B2E0567F53036AC784CD2 (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___cellPosition0, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.GridLayout::LocalToWorld(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  GridLayout_LocalToWorld_mE1FC504F4A2EDD008836B0E907D228BA544D43C5 (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___localPosition0, const RuntimeMethod* method);
// System.Void UnityEngine.GridLayout::CellToLocalInterpolated_Injected(UnityEngine.Vector3&,UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_CellToLocalInterpolated_Injected_m9937AFA062E97FFEBC7E223E17AD50185D1D597E (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___cellPosition0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___ret1, const RuntimeMethod* method);
// System.Void UnityEngine.GridLayout::CellToWorld_Injected(UnityEngine.Vector3Int&,UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_CellToWorld_Injected_m5209A126E47E50A4CAB7E87FFF2653FB587F9DAD (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * ___cellPosition0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___ret1, const RuntimeMethod* method);
// System.Void UnityEngine.GridLayout::WorldToCell_Injected(UnityEngine.Vector3&,UnityEngine.Vector3Int&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_WorldToCell_Injected_m69EB332E1C01D8F601A12ECD6505C98958883546 (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___worldPosition0, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * ___ret1, const RuntimeMethod* method);
// System.Void UnityEngine.GridLayout::LocalToWorld_Injected(UnityEngine.Vector3&,UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_LocalToWorld_Injected_m70720C136D729A64FCA608056FFCD66B911E6719 (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___localPosition0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___ret1, const RuntimeMethod* method);
// System.Void UnityEngine.GridLayout::GetLayoutCellCenter_Injected(UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_GetLayoutCellCenter_Injected_m6CFE62460CF09A38FE3AF6088BD723203B3007E0 (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___ret0, const RuntimeMethod* method);
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// UnityEngine.Vector3 UnityEngine.Grid::GetCellCenterWorld(UnityEngine.Vector3Int)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Grid_GetCellCenterWorld_mE691618405B70E63E29EE21E5512338701C6A4E5 (Grid_t9989A4B9E81E8D350DF09DE256FF140F22AB6476 * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Grid_GetCellCenterWorld_mE691618405B70E63E29EE21E5512338701C6A4E5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  L_0 = ___position0;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_1 = Vector3Int_op_Implicit_m8403835CB506B9EAA8510C113EDB537455F05CC3(L_0, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_2 = GridLayout_GetLayoutCellCenter_m0012793E35AF6CAA364242BF669F9A908E5F8ADD(__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_3 = Vector3_op_Addition_m929F9C17E5D11B94D50B4AFF1D730B70CB59B50E(L_1, L_2, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = GridLayout_CellToLocalInterpolated_m7F44E7F9CEE03E76CD5B2E0567F53036AC784CD2(__this, L_3, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = GridLayout_LocalToWorld_mE1FC504F4A2EDD008836B0E907D228BA544D43C5(__this, L_4, /*hidden argument*/NULL);
		V_0 = L_5;
		goto IL_0021;
	}

IL_0021:
	{
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_6 = V_0;
		return L_6;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// UnityEngine.Vector3 UnityEngine.GridLayout::CellToLocalInterpolated(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  GridLayout_CellToLocalInterpolated_m7F44E7F9CEE03E76CD5B2E0567F53036AC784CD2 (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___cellPosition0, const RuntimeMethod* method)
{
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		GridLayout_CellToLocalInterpolated_Injected_m9937AFA062E97FFEBC7E223E17AD50185D1D597E(__this, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&___cellPosition0), (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_0), /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = V_0;
		return L_0;
	}
}
// UnityEngine.Vector3 UnityEngine.GridLayout::CellToWorld(UnityEngine.Vector3Int)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  GridLayout_CellToWorld_m6701383E4DB6C327D1FA077A1540F4F9B75A1B8D (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___cellPosition0, const RuntimeMethod* method)
{
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		GridLayout_CellToWorld_Injected_m5209A126E47E50A4CAB7E87FFF2653FB587F9DAD(__this, (Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 *)(&___cellPosition0), (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_0), /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = V_0;
		return L_0;
	}
}
// UnityEngine.Vector3Int UnityEngine.GridLayout::WorldToCell(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  GridLayout_WorldToCell_m76149949D04821B4B685B031EBAA7B026E6507FA (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___worldPosition0, const RuntimeMethod* method)
{
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		GridLayout_WorldToCell_Injected_m69EB332E1C01D8F601A12ECD6505C98958883546(__this, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&___worldPosition0), (Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 *)(&V_0), /*hidden argument*/NULL);
		Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  L_0 = V_0;
		return L_0;
	}
}
// UnityEngine.Vector3 UnityEngine.GridLayout::LocalToWorld(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  GridLayout_LocalToWorld_mE1FC504F4A2EDD008836B0E907D228BA544D43C5 (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___localPosition0, const RuntimeMethod* method)
{
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		GridLayout_LocalToWorld_Injected_m70720C136D729A64FCA608056FFCD66B911E6719(__this, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&___localPosition0), (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_0), /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = V_0;
		return L_0;
	}
}
// UnityEngine.Vector3 UnityEngine.GridLayout::GetLayoutCellCenter()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  GridLayout_GetLayoutCellCenter_m0012793E35AF6CAA364242BF669F9A908E5F8ADD (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, const RuntimeMethod* method)
{
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		GridLayout_GetLayoutCellCenter_Injected_m6CFE62460CF09A38FE3AF6088BD723203B3007E0(__this, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_0), /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = V_0;
		return L_0;
	}
}
// System.Void UnityEngine.GridLayout::DoNothing()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_DoNothing_m0ED53398C0D9944BB9666DD66AD9940DE931A54C (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, const RuntimeMethod* method)
{
	{
		return;
	}
}
// System.Void UnityEngine.GridLayout::CellToLocalInterpolated_Injected(UnityEngine.Vector3&,UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_CellToLocalInterpolated_Injected_m9937AFA062E97FFEBC7E223E17AD50185D1D597E (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___cellPosition0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___ret1, const RuntimeMethod* method)
{
	typedef void (*GridLayout_CellToLocalInterpolated_Injected_m9937AFA062E97FFEBC7E223E17AD50185D1D597E_ftn) (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *);
	static GridLayout_CellToLocalInterpolated_Injected_m9937AFA062E97FFEBC7E223E17AD50185D1D597E_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (GridLayout_CellToLocalInterpolated_Injected_m9937AFA062E97FFEBC7E223E17AD50185D1D597E_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.GridLayout::CellToLocalInterpolated_Injected(UnityEngine.Vector3&,UnityEngine.Vector3&)");
	_il2cpp_icall_func(__this, ___cellPosition0, ___ret1);
}
// System.Void UnityEngine.GridLayout::CellToWorld_Injected(UnityEngine.Vector3Int&,UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_CellToWorld_Injected_m5209A126E47E50A4CAB7E87FFF2653FB587F9DAD (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * ___cellPosition0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___ret1, const RuntimeMethod* method)
{
	typedef void (*GridLayout_CellToWorld_Injected_m5209A126E47E50A4CAB7E87FFF2653FB587F9DAD_ftn) (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 *, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *);
	static GridLayout_CellToWorld_Injected_m5209A126E47E50A4CAB7E87FFF2653FB587F9DAD_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (GridLayout_CellToWorld_Injected_m5209A126E47E50A4CAB7E87FFF2653FB587F9DAD_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.GridLayout::CellToWorld_Injected(UnityEngine.Vector3Int&,UnityEngine.Vector3&)");
	_il2cpp_icall_func(__this, ___cellPosition0, ___ret1);
}
// System.Void UnityEngine.GridLayout::WorldToCell_Injected(UnityEngine.Vector3&,UnityEngine.Vector3Int&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_WorldToCell_Injected_m69EB332E1C01D8F601A12ECD6505C98958883546 (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___worldPosition0, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * ___ret1, const RuntimeMethod* method)
{
	typedef void (*GridLayout_WorldToCell_Injected_m69EB332E1C01D8F601A12ECD6505C98958883546_ftn) (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 *);
	static GridLayout_WorldToCell_Injected_m69EB332E1C01D8F601A12ECD6505C98958883546_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (GridLayout_WorldToCell_Injected_m69EB332E1C01D8F601A12ECD6505C98958883546_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.GridLayout::WorldToCell_Injected(UnityEngine.Vector3&,UnityEngine.Vector3Int&)");
	_il2cpp_icall_func(__this, ___worldPosition0, ___ret1);
}
// System.Void UnityEngine.GridLayout::LocalToWorld_Injected(UnityEngine.Vector3&,UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_LocalToWorld_Injected_m70720C136D729A64FCA608056FFCD66B911E6719 (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___localPosition0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___ret1, const RuntimeMethod* method)
{
	typedef void (*GridLayout_LocalToWorld_Injected_m70720C136D729A64FCA608056FFCD66B911E6719_ftn) (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *);
	static GridLayout_LocalToWorld_Injected_m70720C136D729A64FCA608056FFCD66B911E6719_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (GridLayout_LocalToWorld_Injected_m70720C136D729A64FCA608056FFCD66B911E6719_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.GridLayout::LocalToWorld_Injected(UnityEngine.Vector3&,UnityEngine.Vector3&)");
	_il2cpp_icall_func(__this, ___localPosition0, ___ret1);
}
// System.Void UnityEngine.GridLayout::GetLayoutCellCenter_Injected(UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GridLayout_GetLayoutCellCenter_Injected_m6CFE62460CF09A38FE3AF6088BD723203B3007E0 (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___ret0, const RuntimeMethod* method)
{
	typedef void (*GridLayout_GetLayoutCellCenter_Injected_m6CFE62460CF09A38FE3AF6088BD723203B3007E0_ftn) (GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *);
	static GridLayout_GetLayoutCellCenter_Injected_m6CFE62460CF09A38FE3AF6088BD723203B3007E0_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (GridLayout_GetLayoutCellCenter_Injected_m6CFE62460CF09A38FE3AF6088BD723203B3007E0_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.GridLayout::GetLayoutCellCenter_Injected(UnityEngine.Vector3&)");
	_il2cpp_icall_func(__this, ___ret0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
