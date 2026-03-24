from PIL import Image
import pyautogui
import keyboard
pyautogui.PAUSE = 0.003
path = input("Image path: ")
image = Image.open(path)
gray_img = image.convert("L")
bw_img = gray_img.point(lambda x: 0 if x < 128 else 255, '1')
final_img = bw_img.resize((180, 180))
stop_flag = False
def stop():
    global stop_flag
    stop = True

keyboard.add_hotkey('esc', stop)

for x in range(final_img.width):
    if stop_flag: break
    for y in range(final_img.height):
        if stop_flag: break
        if final_img.getpixel((x, y)) == 255:
            pyautogui.click(300+x, 300+y)