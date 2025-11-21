# Generating PWA Icons

To generate PWA icons for the SmartGarden app, use the following methods:

## Option 1: Using PWA Asset Generator (Recommended)

1. Install the tool:
```bash
npm install -g pwa-asset-generator
```

2. Create a source SVG icon (512x512 or larger)

3. Generate all icons:
```bash
pwa-asset-generator logo.svg ./public/icons --icon-only --background "#10b981" --padding "10%"
```

## Option 2: Using Online Tools

Visit: https://www.pwabuilder.com/imageGenerator

1. Upload your logo (PNG or SVG, 512x512 recommended)
2. Select green theme color: #10b981
3. Download the generated icon pack
4. Extract to `public/icons/` directory

## Option 3: Manual Creation (Using GIMP/Photoshop)

Create the following sizes manually:
- icon-72x72.png
- icon-96x96.png
- icon-128x128.png
- icon-144x144.png
- icon-152x152.png
- icon-192x192.png
- icon-384x384.png
- icon-512x512.png

**Design Guidelines:**
- Use a leaf or plant icon on green background (#10b981)
- Keep the design simple and recognizable
- Use 10-15% padding around the icon
- Save as PNG with transparency

## Temporary Placeholder

Until icons are generated, create a simple placeholder:

```bash
# Create a simple green square as placeholder (requires ImageMagick)
for size in 72 96 128 144 152 192 384 512; do
  convert -size ${size}x${size} xc:#10b981 \
    -pointsize $((size/2)) -fill white -gravity center \
    -annotate +0+0 "🌱" \
    icon-${size}x${size}.png
done
```

## Result

After generation, your `public/icons/` folder should contain:
```
icons/
├── icon-72x72.png
├── icon-96x96.png
├── icon-128x128.png
├── icon-144x144.png
├── icon-152x152.png
├── icon-192x192.png
├── icon-384x384.png
└── icon-512x512.png
```

These icons will be used for:
- Home screen installation
- Splash screens
- App shortcuts
- Notifications
- Favicon
