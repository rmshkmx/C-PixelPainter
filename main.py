import keyboard
import os
import config
from core import image_loader, area_selector, painter

path = input("Image path: ")

is_invert = True if input("invert image? (1 - yes, default - no): ") == "1" else False

img = image_loader.load_image(path, is_invert, config.THRESHOLD)

upper_left, lower_right = area_selector.select_area()

between_clicks_delay = config.DEFAULT_DELAY
between_clicks_delay_input = input("between clicks delay in seconds (default: 0.003): ")
if between_clicks_delay_input: between_clicks_delay = float(between_clicks_delay_input) #setting delay

os.system('cls')
print(f"start X: {upper_left[0]}; start Y: {upper_left[1]}")
print(f"size X: {lower_right[0]-upper_left[0]}; size Y: {lower_right[1]-upper_left[1]}") #printing information
print(f"delay: {between_clicks_delay}")
print(f"threshold: {config.THRESHOLD}")
print("escape - break")
print("press enter to start...")
keyboard.wait('enter')

painter.paint(img, between_clicks_delay, upper_left, lower_right)