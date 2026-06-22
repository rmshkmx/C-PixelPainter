import tkinter as tk
from tkinter import filedialog


def run():
    def choose_file():
        file_path_label.config(text=filedialog.askopenfilename(filetypes=[("Images", "*.jpg;*.jpeg;*.png")]))

    main = tk.Tk()
    main.title("PixelPainter")
    main.geometry("400x300")

    label = tk.Label(main, text="PixelPainter")
    label.pack(pady=10)

    file_path_label = tk.Label(main, text="")
    file_path_label.pack()

    open_file_button = tk.Button(main, text="Choose file", command=choose_file)
    open_file_button.pack(pady=5)

    is_invert_checkmark = tk.Checkbutton(main, text="Invert image")
    is_invert_checkmark.pack(pady=5)

    delay_text = tk.Label(main, text="Between clicks delay")
    delay_text.pack()
    delay = tk.Entry(main)
    delay.pack()

    area_label = tk.Label(main, text="upper left: x=0, y=0\nlower right: x=0, y=0")
    area_label.pack(pady=5)
    select_area_button = tk.Button(main, text="Select area")
    select_area_button.pack(pady=5)

    start_button = tk.Button(main, text="Start")
    start_button.pack(pady=10)
    main.mainloop()

if __name__ == "__main__": run()