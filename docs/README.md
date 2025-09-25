# Magnett Automation Core Documentation

This directory contains the documentation for Magnett Automation Core, built with Jekyll and GitHub Pages.

## Structure

- `index.md` - Main landing page
- `getting-started/` - Installation and quick start guides
- `architecture/` - System architecture and design
- `examples/` - Real-world examples and patterns
- `contributing/` - Contribution guidelines
- `_config.yml` - Jekyll configuration

## Multi-language Support

The documentation is structured to support multiple languages in the future:

```
docs/
├── en/          # English (default)
├── es/          # Spanish
├── fr/          # French
└── ...
```

## Local Development

To run the documentation locally:

```bash
cd docs
bundle install
bundle exec jekyll serve
```

Visit `http://localhost:4000` to view the documentation.

## Deployment

The documentation is automatically deployed to GitHub Pages when changes are pushed to the `main` branch.
