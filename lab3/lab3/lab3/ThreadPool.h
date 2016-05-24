#pragma once
#include "Worker.h"
#include <functional>
#include <thread>
#include <queue>
#include <mutex>
#include <memory>
#include <condition_variable>
#include "frametile.h"
#include "model.h"

class ThreadPool
{
public:

	ThreadPool(size_t threadsCount = 1)
	{
		if (threadsCount == 0)
		{
			threadsCount = 1;
		}
		for (size_t i = 0; i < threadsCount; i++)
		{
			workerPtr pWorker(new Worker);
			m_workers.push_back(pWorker);
		}
	}

	~ThreadPool(){}

	template<class _FN, class... _ARGS>
	void RunAsync(_FN _fn, _ARGS... _args)
	{
		GetFreeWorker()->AppendFunction(std::bind(_fn, _args...));
	}

private:
	std::vector<workerPtr> m_workers;

	workerPtr GetFreeWorker()
	{
		workerPtr pWorker;

		size_t minTasks = UINT32_MAX;

		for (auto &it : m_workers)
		{
			if (it->IsEmpty())
			{
				return it;
			}
			else if (minTasks > it->GetTaskCount())
			{
				minTasks = it->GetTaskCount();
				pWorker = it;
			}
		}
		return pWorker;
	}
};

