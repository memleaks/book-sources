(defvar insert-time-format "%X"
  "*Format for \\[insert-time] (c.f. `format-time-string').")

(defvar insert-date-format "%x"
  "*Format for \\[insert-date] (c.f. `format-time-string').")

(defun insert-time ()
  "Insert the current time according to insert-time-format."
  (interactive "*")
  (insert (format-time-string insert-time-format
                              (current-time))))

(defun insert-date ()
  "Insert the current date according to insert-date-format."
  (interactive "*")
  (insert (format-time-string insert-date-format
                              (current-time))))

(defvar writestamp-format "%C"
  "*Format for writestamps (c.f. `format-time-string').")

(defvar writestamp-prefix "WRITESTAMP(("
  "*Unique string identifying start of writestamp.")

(defvar writestamp-suffix "))"
  "*String that terminates a writestamp.")

(defun update-writestamps ()
  "Find writestamps and replace them with the current time."
  (save-excursion
    (save-restriction
      (save-match-data
        (widen)
        (goto-char (point-min))
        (let ((regexp (concat "^"
                              (regexp-quote writestamp-prefix)
                              "\\(.*\\)"
                              (regexp-quote writestamp-suffix)
                              "$")))
          (while (re-search-forward regexp nil t)
            (replace-match (format-time-string writestamp-format
                                               (current-time))
                           t t nil 1))))))
  nil)

(defvar last-change-time nil
  "Time of last buffer modification.")
(make-variable-buffer-local 'last-change-time)

(defun remember-change-time (&rest unused)
  "Store the current time in `last-change-time'."
  (setq last-change-time (current-time)))

(defvar modifystamp-format "%C"
  "*Format for modifystamps (c.f. `format-time-string').")

(defvar modifystamp-prefix "MODIFYSTAMP(("
  "*String identifying start of modifystamp.")

(defvar modifystamp-suffix "))"
  "*String that terminates a modifystamp.")

(defun maybe-update-modifystamps ()
  "Call `update-modifystamps' if the buffer has been modified."
  (if last-change-time ; instead of testing (buffer-modified-p)
      (update-modifystamps)))

(defun update-modifystamps (time)
  "Find modifystamps and replace them with the given time."
  (save-excursion
    (save-restriction
      (save-match-data
        (widen)
        (goto-char (point-min))
        (let ((regexp (concat "^"
                              (regexp-quote modifystamp-prefix)
                              "\\(.*\\)"
                              (regexp-quote modifystamp-suffix)
                              "$")))
          (while (re-search-forward regexp nil t)
            (replace-match (format-time-string modifystamp-format
                                               time)
                           t t nil 1))))))
  (setq last-change-time nil)
  nil)

(provide 'timestamp)
