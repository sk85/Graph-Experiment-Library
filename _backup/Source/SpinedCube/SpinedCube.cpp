

#include "..\..\Header\SpinedCube\SpinedCube.h"

int SpinedCube::CalcDiameter() {
	return static_cast<int>(ceil(static_cast<double>(this->Dimension) / 3) + 3);
}

ulong SpinedCube::GetNeighbor(addr s, int index) {
	addr d;
	if (index > 3) {
		addr type = s & 0b11;
		d = (0b100 | type) << (index - 2);
	}
	else if (index > 1) {
		addr type = s & 1;
		d = (0b10 | type) << (index - 1);
	}
	else {
		d = static_cast<addr>(1) << index;
	}
	return d ^ s;
}

int SpinedCube::CalcDistance(uint32_t s, uint32_t d) {
	return SPR::GetMinimalExpansion(s, d, this->Dimension).GetCount();
}