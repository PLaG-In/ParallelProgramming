#pragma once
#include <functional>
#include <condition_variable>
#include <queue>
#include <mutex>
#include <memory>
#include <thread>

typedef std::function<void()> func;

class Worker
{
public:
	Worker();
	~Worker();
	void AppendFunction(func fn);
	size_t GetTaskCount();
	bool IsEmpty();
private:
	bool m_enabled;
	std::condition_variable m_conditionVariable;
	std::queue<func> m_functionQueue;
	std::mutex m_mutex;
	std::thread m_thread;
	void ThreadFunction();
};

typedef std::shared_ptr<Worker> workerPtr;

