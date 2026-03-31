import React, { useState } from 'react';
import { motion } from 'framer-motion';
import ReactionPicker from './ReactionPicker';
import CommentForm from './CommentForm';

const CommentItem = ({ comment, onToggleReaction, isExpert, lessonId }) => {
    const [showReplyForm, setShowReplyForm] = useState(false);

    const getInitials = (name) => {
        if (!name) return 'U';
        return name.split(' ').map(n => n[0]).join('').toUpperCase();
    };

    const reactionEmojis = {
        '👍': 'Thích',
        '❤️': 'Yêu thích',
        '😂': 'Haha',
        '😮': 'Ngạc nhiên',
        '😢': 'Buồn'
    };

    return (
        <motion.div
            layout
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95 }}
            className={`comment-item ${comment.parentId ? 'mb-3 pb-0 border-0' : ''}`}
            data-comment-id={comment.id}
        >
            <div className={`comment-avatar ${comment.isInstructor ? 'bg-dark' : ''}`}>
                {getInitials(comment.userName)}
            </div>
            <div className="flex-grow-1">
                <div className="comment-meta d-flex justify-content-between align-items-center">
                    <div>
                        <span className="comment-user">{comment.userName}</span>
                        {comment.isInstructor && (
                            <span className="badge bg-dark ms-2" style={{ fontSize: '0.7rem' }}>Chuyên gia</span>
                        )}
                    </div>
                    <span className="comment-date">{comment.createdAt}</span>
                </div>
                <div className="comment-text">{comment.content}</div>

                {/* Reactions Area */}
                <div className="mt-2 d-flex align-items-center gap-2">
                    <ReactionPicker
                        currentReaction={comment.currentUserReaction}
                        onSelect={(type) => onToggleReaction(comment.id, type)}
                    />

                    <div className="d-flex gap-1">
                        {comment.reactions && comment.reactions.map(r => (
                            <span key={r.type} className="badge rounded-pill bg-light text-dark border" title={reactionEmojis[r.type]}>
                                {r.type} {r.count}
                            </span>
                        ))}
                    </div>

                    <button
                        className={`btn btn-sm ${comment.parentId ? 'btn-link text-muted p-0 border-0' : 'btn-outline-primary rounded-pill'} ms-auto d-flex align-items-center gap-1`}
                        style={{ fontSize: '0.8rem', padding: !comment.parentId ? '0.25rem 0.75rem' : '0' }}
                        onClick={() => setShowReplyForm(!showReplyForm)}
                    >
                        <i className={`bi ${comment.parentId ? 'bi-reply' : 'bi-reply-fill'}`}></i> Trả lời
                    </button>
                </div>

                {showReplyForm && (
                    <CommentForm
                        lessonId={lessonId}
                        parentId={comment.id}
                        onCancel={() => setShowReplyForm(false)}
                    />
                )}

                {/* Replies Area */}
                {comment.replies && comment.replies.length > 0 && (
                    <div className="mt-3 ps-4 border-start reply-list">
                        {comment.replies.map(reply => (
                            <CommentItem
                                key={reply.id}
                                comment={reply}
                                onToggleReaction={onToggleReaction}
                                isExpert={isExpert}
                                lessonId={lessonId}
                            />
                        ))}
                    </div>
                )}
            </div>
        </motion.div>
    );
};

export default CommentItem;
