#pragma once

#include <random>

#include "Graph\SpinedCube.h"

namespace Sample {
	void check_CalcDistance(const int minDim, const int MaxDim);

	void RoutingExperiment(SGraph *g, int times, char* filename);
}