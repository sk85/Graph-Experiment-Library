#include <iostream>
#include <string>

#include "Common.h"



void showBinary(addr a) {
	for (int i = 31; i >= 0; i--)
	{
		std::cout << ((a & (1 << i)) >> i);
		if (i % 4 == 0) std::cout << ' ';
	}
	std::cout << std::endl;
}

