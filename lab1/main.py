import string
import threading
import random


ROWS_COUNT = 10
LETTERS_IN_ROW_COUNT = 500
all_letters = []
threads = []
global thread_count


def fill_dict():
    for letter in string.ascii_lowercase:
        all_letters.append([letter, ROWS_COUNT])


def run_sync_func(user_input):
    if user_input == '0':
        return wo_sync_run()
    elif user_input == '1':
        return lock_run()
    elif user_input == '2':
        return semaphore_run()
    elif user_input == '3':
        return event_run()
    else:
        return print_err_msg()


def print_err_msg():
    print('Wrong input')


def decrement(index):
    all_letters[index][1] -= 1
    if all_letters[index][1] == 0:
        del(all_letters[index])


def init_threads():
    for i in range(thread_count):
        threads.append([threading.Thread, False])


def get_free_thread():
    for i in range(thread_count):
        if not threads[i][1]:
            return i
    return -1


def launch_threads(targetf):
    init_threads()
    while len(all_letters) > 0:
        free_thread = get_free_thread()
        while free_thread == -1:
            free_thread = get_free_thread()
        index = random.randint(0, len(all_letters) - 1)
        threads[free_thread][0] = threading.Thread(target=targetf, args=(all_letters[index][0], free_thread,))
        decrement(index)
        threads[free_thread][1] = True
        threads[free_thread][0].start()


def wo_sync_run():
    launch_threads(print_row)


def lock_run():
    global lock
    lock = threading.Lock()
    launch_threads(print_row_lock)


def semaphore_run():
    global semaphore
    semaphore = threading.BoundedSemaphore(1)
    launch_threads(print_row_semaphore)


def init_threads_lock():
    for i in range(thread_count):
        threads.append([threading.Thread, False, threading.Event, threading.Event])


def event_run():
    init_threads()
    event_last = threading.Event()
    is_first_run = True
    while len(all_letters) > 0:
        free_thread = get_free_thread()
        while free_thread == -1:
            free_thread = get_free_thread()
        index = random.randint(0, len(all_letters) - 1)
        event = threading.Event()
        threads[free_thread][0] = threading.Thread(target=print_row_event, args=(all_letters[index][0], event_last,
                                                                                 event, free_thread,))
        if is_first_run:
            event_last.set()
        event_last = event
        decrement(index)
        threads[free_thread][1] = True
        threads[free_thread][0].start()


def print_row(letter, thread_index):
    for i in range(LETTERS_IN_ROW_COUNT):
        print(letter, end='')
    print()
    threads[thread_index][1] = False


def print_row_lock(letter, thread_index):
    with lock:
        print_row(letter, thread_index)


def print_row_semaphore(letter, thread_index):
    with semaphore:
        print_row(letter, thread_index)


def print_row_event(letter, event_wait, event_set, thread_index):
    event_wait.wait()
    event_wait.clear()
    print_row(letter, thread_index)
    event_set.set()


if __name__ == "__main__":
    mode = input('Enter thread sync type (0 == none, 1 == Lock, 2 == Semaphore, 3 == Event) ===> ')
    thread_count = int(input('Enter number of threads ===> '))
    fill_dict()
    run_sync_func(mode)