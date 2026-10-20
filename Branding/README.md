# Sonnenberg icon sources

The vector sources preserve the original idea: the Säntis ridge, its summit tower, and a low sun.

- `sonnenberg-app-icon.svg` is the framed application and installer icon.
- `sonnenberg-context-icon.svg` is the transparent, high-contrast Explorer context-menu mark.

The checked-in `.ico` files contain 16, 20, 24, 32, 40, 48, 64, 72, 96, 128, 180, and 256 pixel frames. The light outline around the context-menu mountain is intentional so that the same asset remains legible in both light and dark Windows themes.

To regenerate the Windows resources with ImageMagick:

```sh
magick -background none sonnenberg-app-icon.svg -define icon:auto-resize=256,180,128,96,72,64,48,40,32,24,20,16 sonnenberg-app-icon.ico
magick -background none sonnenberg-context-icon.svg -define icon:auto-resize=256,180,128,96,72,64,48,40,32,24,20,16 sonnenberg-context-icon.ico
```
