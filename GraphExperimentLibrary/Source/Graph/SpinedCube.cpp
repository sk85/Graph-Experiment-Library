

#include <Graph\SpinedCube.h>
#include <Graph\SpinedCube_shortestpathrouting\SPR.h>

int SpinedCube::CalcDiameter() {
	return static_cast<int>(ceil(static_cast<double>(this->Dimension) / 3) + 3);
}

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