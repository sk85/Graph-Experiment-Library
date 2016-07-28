// Super Class for cubic graphs
/*
	�L���[�u�n�O���t�p�̊�{�N���X
		�E�p�����Ďg��
			�E
		�E�ȉ��̃��\�b�h���p��
			�E�̏ᒸ�_�W���������_������
				�E�C�ӂ̌̏�x�ł���
			�E2���_�Ԃ̋����𕝗D��Ōv�Z
				�E�̏ᒸ�_�W����^����Ό̏���l�����Čv�Z
				�E��A���ȃm�[�h�̋�����-1�ŕԂ����̂ŁA2���_���A�����ۂ����ȒP�Ƀ`�F�b�N�\
		�E����̗\��
			�E�e�X�g���̃��W���[����
				

	�g����
		�E���ׂĂ̏������z�֐����I�[�o�[���C�h����
			�EGetNeighbor��CalcDiameter����
			�EDiameter�͑��Ŏg���ĂȂ��̂ŏ���������Ă���������
		�E���̑��̉��z�֐����ł���΃I�[�o�[���C�h����
			�E���̂Ƃ�CalcDistance����
			�E�����I�ɕ��D��T���̃��\�b�h���Ă�ł�̂ŁA�����I�Ȏ�@����Ă���Ă���Ȃ炻����������Ă������ق�������
		�ESetDimension�Ŏ�����^���Ă��瑼�̃��\�b�h���g��
			�ESetDimension���Ȃ����đ��̎����ł������Ǝg����
*/

#include "..\Header\SGraph.h"

void SGraph::SetDimension(int _dim) {
	this->Dimension = _dim;
	this->NodeNum = 1 << this->Dimension;
}

int SGraph::GetDimension() {
	return this->Dimension;
}

void SGraph::SetRandSeed(int seed)
{
	mt.seed(seed);
}

int SGraph::CalcDistance(uint32_t node1, uint32_t node2) {
	int* distanceTable = CalcAllDistanceBFS(node1);
	ulong distance = distanceTable[node2];
	delete[] distanceTable;
	return distance;
}

int SGraph::CalcDistance(const bool *faults, uint32_t node1, uint32_t node2) {
	int* distanceTable = CalcAllDistanceBFS(faults, node1);
	ulong distance = distanceTable[node2];
	delete[] distanceTable;
	return distance;
}

uint32_t SGraph::GetNodeNum()
{
	return this->NodeNum;
}



int* SGraph::CalcAllDistanceBFS(uint32_t node)
{
	bool *faults = CreateFaults(0);
	int *distance = CalcAllDistanceBFS(faults, node);
	delete[] faults;
	return distance;
}

int* SGraph::CalcAllDistanceBFS(const bool *faults, uint32_t node)
{
	// �����̕\�̏���
	int *distance = new int[this->NodeNum];
	for (size_t i = 0; i < this->NodeNum; i++)
	{
		distance[i] = 100000;
	}
	// �L���[�̏���
	std::queue<uint32_t> que;

	// �T���{��
	que.push(node);	// �n�_���L���[�ɓ����
	distance[node] = 0;
	while (!que.empty())
	{
		uint32_t current = que.front();	// �L���[����1���o����current�Ƃ���
		que.pop();
		for (int i = 0; i < this->Dimension; i++)
		{
			uint32_t neighbor = GetNeighbor(current, i);
			if (faults[neighbor])
			{
				continue;
			}
			else if (distance[neighbor] > distance[current])
			{
				distance[neighbor] = distance[current] + 1;
				que.push(neighbor);
			}
		}
	}

	for (size_t i = 0; i < this->NodeNum; i++)
	{
		if (distance[i] == 100000)
		{
			distance[i] = -1;
		}
	}

	return distance;
}

bool* SGraph::CreateFaults(int faultRatio)
{
	uint32_t faultNum = (uint32_t)(this->NodeNum * ((double)faultRatio / 100));

	// �̏�m�[�h�ۑ��p�z��̏���
	bool *faults = new bool[this->NodeNum];
	for (uint32_t i = 0; i < this->NodeNum; ++i)
	{
		faults[i] = false;
	}

	// �����_���Ɍ̏�����
	for (uint32_t i = 0; i < faultNum; ++i)
	{
		uint32_t rand = this->mt() % (this->NodeNum - i);

		uint32_t index = 0, count = 0;
		while (count <= rand)
		{
			if (!faults[index++])
			{
				count++;
			}
		}
		faults[index - 1] = true;
	}

	return faults;
}

uint32_t SGraph::GetNodeRandom(bool *faults, uint32_t faultRatio)
{
	uint32_t unfaultNum = (uint32_t)(this->NodeNum * ((double)(100 - faultRatio) / 100));
	uint32_t rand = this->mt() % unfaultNum;
	uint32_t index = 0, count = 0;
	while (count <= rand)
	{
		if (!faults[index++])
		{
			count++;
		}
	}
	return index - 1;
}

uint32_t SGraph::GetConnectedNode(bool *faults, uint32_t node) 
{
	int* length = CalcAllDistanceBFS(faults, node);

	// �A���ȃm�[�h���𐔂���
	uint32_t count = 0;
	for (uint32_t i = 0; i < this->NodeNum; ++i)
	{
		if (length[i] > 0) 
		{
			count++;
		}
	}

	uint32_t index = 0;
	if (count > 0)	// �A���ȃm�[�h�����݂��Ȃ��Ȃ�Ύ��s
	{
		// ���Ԗڂ̘A���m�[�h��I�Ԃ�
		uint32_t rand = this->mt() % count;

		// �I�񂾃m�[�h�͉���
		count = 0;
		while (count <= rand)
		{
			if (length[index++] > 0)
			{
				count++;
			}
		}

		delete[] length;
	}
	return index - 1;
}

/*
Experiment* SGraph::CreateExperiment2(int faultRatio)
{
	Experiment* exp = new Experiment(this, faultRatio);
	return exp;
}

*/