import pyautogui, mouse, time

def select_area() -> tuple[tuple[int, int], tuple[int, int]]:
    print("click the upper left corner of the area")
    upper_left = get_corner_coordinates()
    print("click the lower right corner of the area")
    lower_right = get_corner_coordinates()
    return (upper_left, lower_right)

def get_corner_coordinates() -> tuple[int, int]:
    corner = list()
    while(True):
        coordinates_text = f"X: {pyautogui.position().x}; Y: {pyautogui.position().y}"
        print(f"{coordinates_text:<50}", end="\r", flush=True)
        if (mouse.is_pressed(button='left')):
            while mouse.is_pressed(button='left'): time.sleep(0.01)
            corner.append(pyautogui.position().x)
            corner.append(pyautogui.position().y)
            break
    return tuple(corner)