import multiprocessing
import time
import random
import math


RADIUS = 500000
SQR_RADIUS = math.pow(RADIUS, 2)


def get_partition():
    partition = []
    average_iter_count = iter_count // proc_count
    last_iters = iter_count % proc_count
    for i in range(proc_count):
        partition.append(average_iter_count)
    for i in range(last_iters):
        partition[i] += 1
    return partition


def get_montecarlo_hits(count):
    hits = 0
    for i in range(count):
        if math.pow(random.randint(0, RADIUS), 2) + math.pow(random.randint(0, RADIUS), 2) <= SQR_RADIUS:
            hits += 1
    return hits


def handle_process(queue, results):
    current_result = 0
    for count in iter(queue.get, None):
        current_result += get_montecarlo_hits(count)
        queue.task_done()
    queue.task_done()
    results.append(current_result)


def get_pi():
    mp_queue = multiprocessing.JoinableQueue()
    partition = get_partition()
    for it_count in partition:
        mp_queue.put(it_count)
    for p in range(proc_count):
        mp_queue.put(None)
    manager = multiprocessing.Manager()
    result = manager.list()
    processes = []
    for i in range(proc_count):
        processes.append(multiprocessing.Process(target=handle_process, args=(mp_queue, result,)))
        processes[-1].start()
    wait(mp_queue, processes)
    in_circle = 0
    for res in result:
        in_circle += res
    result_pi = in_circle * 4 / iter_count
    return result_pi


def wait(queue, processes):
    queue.join()
    for process in processes:
        process.join()


if __name__ == '__main__':
    random.seed()
    proc_count = int(input('Enter processes count ===> '))
    iter_count = int(input('Enter number of iterations ===> '))
    start_time = time.time()
    pi = get_pi()
    elapsed_time = time.time() - start_time
    print('Pi = {0:.10f}. Elapsed time {1:.3}'.format(pi, elapsed_time))
