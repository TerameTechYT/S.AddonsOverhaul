#pragma once
#include <string>

inline int string_ends_with(const wchar_t* string, const wchar_t* postfix)
{
	const auto string_length = wcslen(string);
	const auto end_string_length = wcslen(postfix);
	const auto string_offset = string_length - end_string_length;
	if (string_length < end_string_length) return false;
	const auto* target_string = string + string_offset;
	return wcscmp(target_string, postfix) == 0;
}

inline int string_ends_with(const char* string, const char* postfix)
{
	const auto string_length = strlen(string);
	const auto end_string_length = strlen(postfix);
	const auto string_offset = string_length - end_string_length;
	if (string_length < end_string_length) return false;
	const auto* target_string = string + string_offset;
	return strcmp(target_string, postfix) == 0;
}
