#include <iostream>
#include <atltime.h>
#include <string>

#include "..\Header\Common.h"



void showBinary(addr a) {
	for (int i = 31; i >= 0; i--)
	{
		std::cout << ((a & (1 << i)) >> i);
		if (i % 4 == 0) std::cout << ' ';
	}
	std::cout << std::endl;
}


std::string getTime() {
	SYSTEMTIME t;
	GetLocalTime(&t);

	return std::to_string(t.wMonth) + "/" + std::to_string(t.wDay) + " " + std::to_string(t.wHour) + ":" + std::to_string(t.wMinute) + ":" + std::to_string(t.wSecond);
}