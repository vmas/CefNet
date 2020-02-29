﻿// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------
// Generated by CefGen
// Source: include/internal/cef_types.h
// --------------------------------------------------------------------------------------------﻿
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
// --------------------------------------------------------------------------------------------

#pragma warning disable 0169, 1591, 1573

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CefNet.WinApi;

namespace CefNet
{
	/// <summary>
	/// The JavaScript type values.
	/// </summary>
	public enum CefV8ValueType
	{
		/// <summary>
		/// The value is an invalid value.
		/// </summary>
		Invalid = 0,

		/// <summary>
		/// The value is the undefined value.
		/// </summary>
		Undefined = 1,

		/// <summary>
		/// The value is the null value.
		/// </summary>
		Null = 2,

		/// <summary>
		/// The value is a JavaScript Boolean value.
		/// </summary>
		Bool = 3,

		/// <summary>
		/// The value is an int value.
		/// </summary>
		Int = 4,

		/// <summary>
		/// The value is an unsigned int value.
		/// </summary>
		UInt = 5,

		/// <summary>
		/// The value is a double value.
		/// </summary>
		Double = 6,

		/// <summary>
		/// The value is a JavaScript Date value.
		/// </summary>
		Date = 7,

		/// <summary>
		/// The value is a JavaScript string value.
		/// </summary>
		String = 8,

		/// <summary>
		/// The value is a JavaScript Symbol value.
		/// </summary>
		Symbol = 9,

		/// <summary>
		/// The value is a bigint.
		/// </summary>
		BigInt = 10,

		/// <summary>
		/// The value is a JavaScript object value.
		/// </summary>
		Object = 11,

		/// <summary>
		/// The object type mask.
		/// </summary>
		ObjectMask = 1 << 16,

		/// <summary>
		/// The value is a JavaScript Boolean object value.
		/// </summary>
		BooleanObject = ObjectMask | Bool,

		/// <summary>
		/// The value is a JavaScript Number object value.
		/// </summary>
		NumberObject = ObjectMask | Double,

		/// <summary>
		/// The value is a JavaScript String object value.
		/// </summary>
		StringObject = ObjectMask | String,

		/// <summary>
		/// The value is a JavaScript Symbol object value.
		/// </summary>
		SymbolObject = ObjectMask | Symbol,

		/// <summary>
		/// The value is a JavaScript BigInt object value.
		/// </summary>
		BigIntObject = ObjectMask | BigInt,

		/// <summary>
		/// The value is a JavaScript object value.
		/// </summary>
		UnknownObject = ObjectMask | Object,

		/// <summary>
		/// The value is a JavaScript function object value.
		/// </summary>
		Function = 65548,

		/// <summary>
		/// The value is a JavaScript array object value.
		/// </summary>
		Array = 65549,

		/// <summary>
		/// The value is a JavaScript ArrayBuffer object value.
		/// </summary>
		ArrayBuffer = 65550,

		/// <summary>
		/// The value is a JavaScript ArrayBufferView object value.
		/// </summary>
		ArrayBufferView = 65551,

		/// <summary>
		/// The value is a JavaScript BigInt64Array object value.
		/// </summary>
		BigInt64Array = 65552,

		/// <summary>
		/// The value is a JavaScript DataView object value.
		/// </summary>
		DataView = 65553,

		/// <summary>
		/// The value is a JavaScript Float32Array object value.
		/// </summary>
		Float32Array = 65554,

		/// <summary>
		/// The value is a JavaScript Float64Array object value.
		/// </summary>
		Float64Array = 65555,

		/// <summary>
		/// The value is a JavaScript Int8Array object value.
		/// </summary>
		Int8Array = 65556,

		/// <summary>
		/// The value is a JavaScript Int16Array object value.
		/// </summary>
		Int16Array = 65557,

		/// <summary>
		/// The value is a JavaScript Int32Array object value.
		/// </summary>
		Int32Array = 65558,

		/// <summary>
		/// The value is a JavaScript Map object value.
		/// </summary>
		Map = 65559,

		/// <summary>
		/// The value is a JavaScript Promise object value.
		/// </summary>
		Promise = 65560,

		/// <summary>
		/// The value is a JavaScript Proxy object value.
		/// </summary>
		Proxy = 65561,

		/// <summary>
		/// The value is a JavaScript RegExp object value.
		/// </summary>
		Regexp = 65562,

		/// <summary>
		/// The value is a JavaScript Set object value.
		/// </summary>
		Set = 65563,

		/// <summary>
		/// The value is a JavaScript WeakMap object value.
		/// </summary>
		WeakMap = 65564,

		/// <summary>
		/// The value is a JavaScript WeakSet object value.
		/// </summary>
		WeakSet = 65565,
	}
}

