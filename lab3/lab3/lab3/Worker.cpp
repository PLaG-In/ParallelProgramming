#include "Worker.h"


Worker::Worker()
	: m_enabled(true)
	, m_functionQueue()
	, m_thread(&Worker::ThreadFunction, this)
{
}


Worker::~Worker()
{
	m_enabled = false;
	m_conditionVariable.notify_one();
	m_thread.join();
}

void Worker::AppendFunction(func fn)
{
	std::unique_lock<std::mutex> locker(m_mutex);
	m_functionQueue.push(fn);
	m_conditionVariable.notify_one();
}

size_t Worker::GetTaskCount()
{
	std::unique_lock<std::mutex> locker(m_mutex);
	return m_functionQueue.size();
}

bool Worker::IsEmpty()
{
	std::unique_lock<std::mutex> locker(m_mutex);
	return m_functionQueue.empty();
}

void Worker::ThreadFunction()
{
	while (m_enabled)
	{
		std::unique_lock<std::mutex> locker(m_mutex);
		// ������� �����������, � �������� ��� ��� �� ������ �����������
		// ����� ������ ���������� ���� ������� �� ������ ���� �� ��������
		m_conditionVariable.wait(locker, [&](){ return !m_functionQueue.empty() || !m_enabled; });
		while (!m_functionQueue.empty())
		{
			func fn = m_functionQueue.front();
			// ������������ ������ ����� ������� ��������
			locker.unlock();
			fn();
			// ���������� ���������� ����� ����� ������� fqueue.empty() 
			locker.lock();
			m_functionQueue.pop();
		}
	}
}