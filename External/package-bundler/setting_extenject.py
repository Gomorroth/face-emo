from pathlib import Path

src_dir = Path('../Extenject/UnityProject/Assets/Plugins/Zenject/Source')
dst_dir = Path('../../Packages/jp.suzuryg.facial-expression-switcher/External/Mathijs-Bakker/Extenject/Runtime')
ignore_dirs = [Path('Editor'),]
target_extensions = ['.cs',]
gitignore_content = '''*.cs
*.meta
!*.asmdef.meta
'''
