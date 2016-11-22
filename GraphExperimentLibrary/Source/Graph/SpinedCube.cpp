

#include "SpinedCube.h"
#include "SpinedCube_shortestpathrouting\SPR.h"

uint32_t SpinedCube::GetNeighbor(uint32_t s, int index) {
	uint32_t d;
	if (index > 3) {
		uint32_t type = s & 0b11;
		d = (0b100 | type) << (index - 2);
	}
	else if (index > 1) {
		uint32_t type = s & 1;
		d = (0b10 | type) << (index - 1);
	}
	else {
		d = static_cast<uint32_t>(1) << index;
	}
	return d ^ s;
}

int SpinedCube::CalcDistance(uint32_t s, uint32_t d) {
	return SPR::GetMinimalExpansion(s, d, this->Dimension).GetCount();
}

int SpinedCube::GetDegree(uint32_t node)
{
	return this->Dimension;
}

uint32_t SpinedCube::CalcNodeNum()
{
	return 1 << this->Dimension;
}