
#include <Graph\SpinedCube.h>

uint32_t SpinedCube::CalcNodeNum()
{
	return 1 << this->Dimension;
}

SNode* SpinedCube::GetNeighbor(SNode* node, int index)
{
	uint32_t d;
	if (index > 3) {
		uint32_t type = node->GetIndex() & 0b11;
		d = (0b100 | type) << (index - 2);
	}
	else if (index > 1) {
		uint32_t type = node->GetIndex() & 1;
		d = (0b10 | type) << (index - 1);
	}
	else {
		d = 1 << index;
	}
	return (SNode*)(new CubeNode(d ^ node->GetIndex()));
}

int SpinedCube::CalcDistance(SNode* s, SNode* d)
{
	return SPR::GetMinimalExpansion(s->GetIndex(), d->GetIndex(), this->Dimension).GetCount();
}

int SpinedCube::GetDegree(SNode *node)
{
	return this->Dimension;
}