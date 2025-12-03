module.exports = {
  extends: ['@commitlint/config-conventional'],
  rules: {
    'type-enum': [
      2,
      'always',
      [
        'feat',
        'fix',
        'docs',
        'chore',
        'refactor',
        'test',
        'perf',
        'build',
        'ci',
        'revert',
        'style'
      ]
    ],
    'subject-max-length': [2, 'always', 100],
    'header-max-length': [2, 'always', 100],
    'scope-empty': [2, 'never'],
    'references-empty': [2, 'never']
  }
};
