import React, { useState, useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import { motion, AnimatePresence } from 'framer-motion';
import CommentItem from './components/CommentItem';
import CommentForm from './components/CommentForm';

const App = () => {
    const [comments, setComments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [lessonId] = useState(parseInt(document.getElementById('qa-root')?.dataset.lessonId || '0'));
    const [userId] = useState(parseInt(document.getElementById('qa-root')?.dataset.userId || '0'));
    const [isExpert] = useState(document.getElementById('qa-root')?.dataset.isExpert === 'True');
    const hubConnection = useRef(null);

    useEffect(() => {
        fetchComments();
        setupSignalR();

        return () => {
            if (hubConnection.current) {
                hubConnection.current.invoke("LeaveLesson", lessonId).catch(() => { });
                hubConnection.current.stop().catch(() => { });
            }
        };
    }, []);

    const fetchComments = async () => {
        try {
            const response = await axios.get(`/Course/GetLessonQa?lessonId=${lessonId}`);
            setComments(response.data);
            setLoading(false);
        } catch (error) {
            console.error("Error fetching comments:", error);
            setLoading(false);
        }
    };

    const setupSignalR = async () => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        connection.on("ReceiveComment", (newComment) => {
            setComments(prev => {
                if (prev.some(c => c.id === newComment.id)) return prev;
                return [newComment, ...prev];
            });
        });

        connection.on("ReceiveReply", (newReply) => {
            setComments(prev => prev.map(c => {
                if (c.id === newReply.parentId) {
                    if (c.replies && c.replies.some(r => r.id === newReply.id)) return c;
                    return { ...c, replies: [...(c.replies || []), newReply] };
                }
                return c;
            }));
        });

        connection.on("UpdateReaction", ({ commentId, reactions }) => {
            setComments(prev => {
                let updated = prev.map(c => {
                    if (c.id === commentId) {
                        return { ...c, reactions };
                    }
                    if (c.replies) {
                        return {
                            ...c,
                            replies: c.replies.map(r => r.id === commentId ? { ...r, reactions } : r)
                        };
                    }
                    return c;
                });
                return updated;
            });
        });

        try {
            await connection.start();
            await connection.invoke("JoinLesson", lessonId).catch(() => { });
            hubConnection.current = connection;
        } catch (err) {
            console.log("SignalR Connection suppressed (Development/Duplicate)");
        }
    };

    const handleToggleReaction = async (commentId, reactionType) => {
        try {
            const response = await axios.post('/Course/ToggleReaction', { commentId, reactionType });
            if (response.data.success) {
                // Local update while waiting for SignalR or as fallback
                const updatedReactions = response.data.reactions;
                setComments(prev => {
                    return prev.map(c => {
                        if (c.id === commentId) {
                            return {
                                ...c,
                                reactions: updatedReactions,
                                currentUserReaction: c.currentUserReaction === reactionType ? null : reactionType
                            };
                        }
                        if (c.replies) {
                            return {
                                ...c,
                                replies: c.replies.map(r => r.id === commentId ? {
                                    ...r,
                                    reactions: updatedReactions,
                                    currentUserReaction: r.currentUserReaction === reactionType ? null : reactionType
                                } : r)
                            };
                        }
                        return c;
                    });
                });
            }
        } catch (error) {
            console.error("Error toggling reaction:", error);
        }
    };

    if (loading) {
        return (
            <div className="flex justify-center items-center h-48">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
            </div>
        );
    }

    return (
        <div className="qa-app">
            <h4 className="fw-bold mb-1">Hỏi đáp với chuyên gia</h4>
            <p className="text-muted mb-4 small">
                Bạn có thắc mắc về bài học này? Hãy để lại câu hỏi và chuyên gia phụ trách khóa học sẽ trả lời.
            </p>

            <CommentForm lessonId={lessonId} />

            <div id="commentList" className="mt-4">
                <AnimatePresence initial={false}>
                    {comments.length > 0 ? (
                        comments.map(comment => (
                            <CommentItem
                                key={comment.id}
                                comment={comment}
                                onToggleReaction={handleToggleReaction}
                                isExpert={isExpert}
                                lessonId={lessonId}
                            />
                        ))
                    ) : (
                        <div className="text-center py-4 no-comments">
                            <p className="text-muted small">Chưa có câu hỏi nào. Hãy là người đầu tiên!</p>
                        </div>
                    )}
                </AnimatePresence>
            </div>
        </div>
    );
};

export default App;
