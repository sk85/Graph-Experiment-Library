#include "Results.h"


void Results::Add(char* name)
{
	Names[Count] = string(name);
	for (size_t i = 0; i < 10; i++)
	{
		SuccessCount[Count][i] = 0;
		TotalSteps[Count][i] = 0;
	}
	Count++;
}

void Results::Update(int id, int faultRatio, int step)
{
	if (step > 0)
	{
		SuccessCount[id][faultRatio]++;
		TotalSteps[id][faultRatio] += step;
	}
}

void Results::Save(char* path, int trials)
{
	ofstream ofs(path);

	ofs << "ŒÌá—¦,";
	for (size_t i = 0; i < 10; i++)
	{
		ofs << i * 10 << "%,";
	}
	ofs << endl;

	for (size_t id = 0; id < Count; id++)
	{
		ofs << Names[id] << "(“ž’B—¦),";
		for (size_t faultRatio = 0; faultRatio < 10; faultRatio++)
		{
			ofs << (double)SuccessCount[id][faultRatio] / trials << ",";
		}
		ofs << endl;
	}

	for (size_t id = 0; id < Count; id++)
	{
		ofs << Names[id] << "(Œo˜H’·),";
		for (size_t faultRatio = 0; faultRatio < 10; faultRatio++)
		{
			ofs << (double)TotalSteps[id][faultRatio] / trials << ",";
		}
		ofs << endl;
	}
	ofs << endl;

	ofs.close();
}