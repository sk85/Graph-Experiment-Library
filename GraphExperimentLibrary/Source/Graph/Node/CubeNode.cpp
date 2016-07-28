#include <Graph\Node\CubeNode.h>


uint32_t CubeNode::GetIndex()
{
	return this->Addr;
}

void CubeNode::SetAddr(uint32_t addr)
{
	this->Addr = addr;
}