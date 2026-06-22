import tkinter as tk
from tkinter import filedialog, messagebox
from core import area_selector, image_loader, painter
import config

def run():
    upper_left = ()
    lower_right = ()
    def choose_file():
        file_path_label.config(text=filedialog.askopenfilename(filetypes=[("Images", "*.jpg;*.jpeg;*.png")]))
    def select_area():
        nonlocal upper_left, lower_right
        upper_left, lower_right = area_selector.select_area()
        area_label.config(text=f"upper left: x={upper_left[0]}, y={upper_left[1]}\nlower right: x={lower_right[0]}, y={lower_right[1]}")
    def paint():
        if not messagebox.askyesno("confirm", "start painting?\nESC - break"):
            return
        if file_path_label.cget('text') == "":
            messagebox.showwarning("Warning", "file field is empty")
            return
        elif delay.get() == "":
            messagebox.showwarning("Warning", "delay field is empty")
            return
        elif area_label.cget('text') == "":
            messagebox.showwarning("Warning", "area field is empty")
            return
        else:
            img = image_loader.load_image(file_path_label.cget('text'), invert_var.get(), config.THRESHOLD)
            painter.paint(img, float(delay.get()), upper_left, lower_right)
    main = tk.Tk()
    main.title("PixelPainter")
    main.geometry("400x300")

    label = tk.Label(main, text="PixelPainter")
    label.pack(pady=10)

    file_path_label = tk.Label(main, text="")
    file_path_label.pack()

    open_file_button = tk.Button(main, text="Choose file", command=choose_file)
    open_file_button.pack(pady=5)

    invert_var = tk.BooleanVar()
    is_invert_checkmark = tk.Checkbutton(main, text="Invert image", variable=invert_var)
    is_invert_checkmark.pack(pady=5)

    delay_text = tk.Label(main, text="Between clicks delay")
    delay_text.pack()
    delay = tk.Entry(main)
    delay.insert(0, config.DEFAULT_DELAY)
    delay.pack()

    area_label = tk.Label(main, text="")
    area_label.pack(pady=5)
    select_area_button = tk.Button(main, text="Select area", command=select_area)
    select_area_button.pack(pady=5)

    start_button = tk.Button(main, text="Start", command=paint)
    start_button.pack(pady=10)
    main.mainloop()