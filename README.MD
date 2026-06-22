# Pixel Painter

convert any image into a pixel-by-pixel mouse painting - automatically

## How it works
1. load an image
2. invert colors if needed
3. select the area where you want to paint
4. adjust delay between clicks
5. press Start to begin, press ESC to stop at any time

## Installation
1. clone the repository
```
git clone https://github.com/ArviPlay/PixelPainter.git
```
2. jump into
```
cd PixelPainter
```
3. create virtual environment
```
python -m venv .venv
```
4. activate it

    - Linux, macOS
    ```
    source .venv/bin/activate
    ```
    - Windows
    ```
    .venv\Scripts\activate
    ```
5. install dependencies
```
pip install -r requirements.txt
```
## Run
```
python main.py
```
## Notes
- works best with high-contrast black-and-white images